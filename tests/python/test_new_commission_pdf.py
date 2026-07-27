from __future__ import annotations

import importlib.util
import tempfile
import unittest
from datetime import date
from pathlib import Path

from pypdf import PdfReader
from reportlab.pdfgen import canvas


ROOT = Path(__file__).resolve().parents[2]
SCRIPT_PATH = ROOT / "tools" / "New-CommissionPdf.py"
SPEC = importlib.util.spec_from_file_location("new_commission_pdf", SCRIPT_PATH)
if SPEC is None or SPEC.loader is None:
    raise RuntimeError(f"Nie można załadować modułu: {SCRIPT_PATH}")
PDF_MODULE = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(PDF_MODULE)


class NewCommissionPdfTests(unittest.TestCase):
    def test_build_pdf_creates_and_validates_complete_document(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            output = Path(directory) / "nested" / "commission.pdf"

            result = PDF_MODULE.build_pdf(
                output_path=output,
                generated_on=date(2026, 7, 27),
            )

            self.assertEqual(output, result)
            self.assertTrue(output.is_file())
            reader = PdfReader(str(output))
            self.assertGreaterEqual(len(reader.pages), 9)
            first_page = reader.pages[0].extract_text() or ""
            self.assertIn("2026-07-27", first_page)

    def test_build_pdf_rejects_missing_fonts_before_rendering(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            missing = Path(directory) / "missing.ttf"

            with self.assertRaisesRegex(FileNotFoundError, "czcionek Arial"):
                PDF_MODULE.build_pdf(font_regular=missing, font_bold=missing)

    def test_build_pdf_rejects_missing_visual_baseline(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            missing = Path(directory) / "missing.png"

            with self.assertRaisesRegex(FileNotFoundError, "zrzutu testu wizualnego"):
                PDF_MODULE.build_pdf(screenshot_path=missing)

    def test_validate_pdf_reports_each_structural_failure(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            too_short = root / "too-short.pdf"
            self._write_pdf(too_short, ["Abituria"])
            with self.assertRaisesRegex(RuntimeError, "zbyt mało stron"):
                PDF_MODULE.validate_pdf(too_short)

            missing_title = root / "missing-title.pdf"
            self._write_pdf(missing_title, ["Inny dokument"] * 9)
            with self.assertRaisesRegex(RuntimeError, "oczekiwanego tytułu"):
                PDF_MODULE.validate_pdf(missing_title)

            missing_sections = root / "missing-sections.pdf"
            self._write_pdf(missing_sections, ["Abituria"] * 9)
            with self.assertRaisesRegex(RuntimeError, "nie zawiera sekcji"):
                PDF_MODULE.validate_pdf(missing_sections)

    @staticmethod
    def _write_pdf(path: Path, pages: list[str]) -> None:
        document = canvas.Canvas(str(path))
        for text in pages:
            document.drawString(72, 720, text)
            document.showPage()
        document.save()


if __name__ == "__main__":
    unittest.main()
