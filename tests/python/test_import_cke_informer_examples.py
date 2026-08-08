from __future__ import annotations

import hashlib
import importlib.util
import io
import json
import sys
import tempfile
import unittest
from contextlib import redirect_stdout
from pathlib import Path
from unittest import mock


ROOT = Path(__file__).resolve().parents[2]
SCRIPT_PATH = ROOT / "tools" / "Import-CkeInformerExamples.py"
SPEC = importlib.util.spec_from_file_location("import_cke_informer_examples", SCRIPT_PATH)
if SPEC is None or SPEC.loader is None:
    raise RuntimeError(f"Nie można załadować modułu: {SCRIPT_PATH}")
PDF_MODULE = importlib.util.module_from_spec(SPEC)
sys.modules[SPEC.name] = PDF_MODULE
SPEC.loader.exec_module(PDF_MODULE)


class FakePage:
    def __init__(self, text: str) -> None:
        self._text = text

    def extract_text(self) -> str:
        return self._text


class FakeReader:
    def __init__(self, pages: list[str]) -> None:
        self.pages = [FakePage(page) for page in pages]


class ImportCkeInformerExamplesTests(unittest.TestCase):
    def test_text_helpers_strip_headers_normalize_pages_and_map_requirements(self) -> None:
        cleaned = PDF_MODULE._clean_page(
            "12 Informator o egzaminie maturalnym test\n"
            "Przykładowe zadania z rozwiązaniami 12\nTreść   \n"
        )
        self.assertEqual(cleaned, "Treść")
        normalized = PDF_MODULE._normalize_transcription(
            "A  \n\n\n<<<PAGE 13>>>\nB\u00adC\n"
        )
        self.assertEqual(normalized, "A\n\nBC")
        requirements = PDF_MODULE._requirement_ids(
            "Wymaganie szczegółowe\n"
            "IIII. Równania i nierówności. Zdający:\n"
            "5R) analizuje równania.\n"
            "I. Liczby rzeczywiste. Zdający:\n"
            "1) wykonuje działania.\n"
            "R) zmienia podstawę.\n"
            "Zasady oceniania\n"
        )
        self.assertEqual(requirements, ["III.E.5", "I.B.1", "I.E.1"])
        self.assertEqual(
            PDF_MODULE._requirement_ids(
                "Wymaganie szczegółowe\n"
                "XIII. Optymalizacja i rachunek różniczkowy.\n"
                "Zdający rozwiązuje zadania optymalizacyjne.\n"
                "Zasady oceniania\n"
            ),
            ["XIII.B.1"],
        )
        self.assertIsNone(PDF_MODULE._requirement_heading_roman("I. Liczby rzeczywiste."))
        self.assertIsNone(PDF_MODULE._requirement_heading_roman("A. Algebra. Zdający:"))
        self.assertIsNone(PDF_MODULE._requirement_token("brak wymagania"))
        self.assertEqual(
            list(PDF_MODULE._requirement_sections("Wymaganie szczegółowe bez zasad")),
            [],
        )

    def test_points_pages_hash_and_argument_helpers_cover_boundaries(self) -> None:
        self.assertEqual(PDF_MODULE._maximum_points("Zadanie 4. (0–3)", 4), 3)
        self.assertEqual(
            PDF_MODULE._maximum_points(
                "Zadanie 8.\nZadanie 8.1. (0-2)\nZadanie 8.2. (0-3)", 8
            ),
            5,
        )
        with self.assertRaisesRegex(ValueError, "punktacji"):
            PDF_MODULE._maximum_points("Zadanie 9.", 9)

        chunks = [PDF_MODULE.PageChunk(12, 10), PDF_MODULE.PageChunk(13, 30)]
        self.assertEqual(PDF_MODULE._source_pages(chunks, 20, 45), [12, 13])

        self.assertEqual(PDF_MODULE._task_start_number("Zadanie 4. (0–3)"), 4)
        self.assertEqual(PDF_MODULE._task_start_number("Zadanie 4. (0-3)"), 4)
        self.assertEqual(PDF_MODULE._task_start_number("Zadanie\t4. (0-3)"), 4)
        self.assertEqual(PDF_MODULE._task_start_number("Zadanie 4."), 4)
        self.assertIsNone(PDF_MODULE._task_start_number("Zadanie4. (0-3)"))
        self.assertIsNone(PDF_MODULE._task_start_number("Zadanie 4.1. (0-3)"))
        self.assertIsNone(PDF_MODULE._task_start_number("Zadanie 4. (0+3)"))
        self.assertEqual(
            list(PDF_MODULE._lines_with_offsets("A\nBC")),
            [(0, "A\n"), (2, "BC")],
        )

        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "source.bin"
            path.write_bytes(b"Abituria")
            self.assertEqual(PDF_MODULE._sha256(path), hashlib.sha256(b"Abituria").hexdigest().upper())
        args = PDF_MODULE._parse_args(
            ["--basic-pdf", "basic.pdf", "--extended-pdf", "extended.pdf"]
        )
        self.assertEqual(args.output, Path("Content/official-course-examples.json"))

    def test_extract_examples_builds_complete_record_and_rejects_bad_inputs(self) -> None:
        source = {
            "id": "test-guide",
            "level": "basic",
            "documentSha256": "A" * 64,
            "firstExamplePage": 1,
            "lastExamplePage": 2,
            "exampleCount": 1,
        }
        pages = [
            "1 Informator o egzaminie maturalnym test\n"
            "Zadanie 1. (0-2)\nPolecenie.\n"
            "Wymaganie szczegółowe\nI. Liczby rzeczywiste. Zdający:\n"
            "1) wykonuje działania.\nZasady oceniania\n2 pkt.\n",
            "Przykładowe zadania z rozwiązaniami 2\n"
            "Przykładowe pełne rozwiązanie\nObliczenia.",
        ]
        with mock.patch.object(PDF_MODULE, "PdfReader", return_value=FakeReader(pages)), mock.patch.object(
            PDF_MODULE, "_sha256", return_value="A" * 64
        ):
            examples = PDF_MODULE._extract_examples(Path("test.pdf"), source)
        example = self.assert_single(examples)
        self.assertEqual(example["id"], "cke-basic-guide-task-01")
        self.assertEqual(example["sourcePages"], [1, 2])
        self.assertEqual(example["requirementIds"], ["I.B.1"])
        self.assertEqual(example["maximumPoints"], 2)
        self.assertIn("Przykładowe pełne rozwiązanie", example["transcription"])

        with mock.patch.object(PDF_MODULE, "_sha256", return_value="B" * 64):
            with self.assertRaisesRegex(ValueError, "SHA-256"):
                PDF_MODULE._extract_examples(Path("test.pdf"), source)

        empty_reader = FakeReader(["Brak zadania", "Brak zadania"])
        with mock.patch.object(PDF_MODULE, "PdfReader", return_value=empty_reader), mock.patch.object(
            PDF_MODULE, "_sha256", return_value="A" * 64
        ):
            with self.assertRaisesRegex(ValueError, "Odczytano 0"):
                PDF_MODULE._extract_examples(Path("test.pdf"), source)

        no_requirement_pages = [
            "Zadanie 1. (0-1)\nPolecenie.\nZasady oceniania\n1 pkt.\nRozwiązanie\nA.",
            "Koniec.",
        ]
        with mock.patch.object(
            PDF_MODULE, "PdfReader", return_value=FakeReader(no_requirement_pages)
        ), mock.patch.object(PDF_MODULE, "_sha256", return_value="A" * 64):
            with self.assertRaisesRegex(ValueError, "mapowania wymagania"):
                PDF_MODULE._extract_examples(Path("test.pdf"), source)

    def test_build_write_and_main_keep_the_catalog_deterministic(self) -> None:
        basic_example = {"id": "basic"}
        extended_example = {"id": "extended"}
        with mock.patch.object(
            PDF_MODULE,
            "_extract_examples",
            side_effect=[[basic_example], [extended_example]],
        ) as extract:
            catalog = PDF_MODULE.build_catalog(Path("b.pdf"), Path("e.pdf"))
        self.assertEqual(extract.call_count, 2)
        self.assertEqual(catalog["examples"], [basic_example, extended_example])
        self.assertEqual(catalog["schemaVersion"], 1)

        with tempfile.TemporaryDirectory() as directory:
            output = Path(directory) / "nested" / "catalog.json"
            PDF_MODULE.write_catalog(catalog, output)
            data = output.read_bytes()
            self.assertTrue(data.endswith(b"\n"))
            self.assertEqual(json.loads(data.decode("utf-8")), catalog)

            with mock.patch.object(PDF_MODULE, "build_catalog", return_value=catalog), mock.patch.object(
                PDF_MODULE, "write_catalog"
            ) as write:
                stream = io.StringIO()
                with redirect_stdout(stream):
                    result = PDF_MODULE.main(
                        [
                            "--basic-pdf",
                            "b.pdf",
                            "--extended-pdf",
                            "e.pdf",
                            "--output",
                            str(output),
                        ]
                    )
            self.assertEqual(result, 0)
            write.assert_called_once_with(catalog, output)
            self.assertIn("2 oficjalnych przykładów", stream.getvalue())

    def test_visual_reference_metadata_is_complete(self) -> None:
        self.assertEqual(sum(len(items) for items in PDF_MODULE.VISUAL_DESCRIPTIONS.values()), 53)
        self.assertEqual(len(PDF_MODULE.VISUAL_DESCRIPTIONS), 39)
        for (level, number), references in PDF_MODULE.VISUAL_DESCRIPTIONS.items():
            self.assertIn(level, {"basic", "extended"})
            self.assertGreater(number, 0)
            for page, description in references:
                self.assertGreater(page, 0)
                self.assertTrue(description.strip())

    def assert_single(self, values: list[dict[str, object]]) -> dict[str, object]:
        self.assertEqual(len(values), 1)
        return values[0]


if __name__ == "__main__":
    unittest.main()
