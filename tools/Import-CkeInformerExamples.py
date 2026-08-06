#!/usr/bin/env python3
"""Import official worked examples from the two CKE Formula 2023 guides.

The generated catalog is a separate sourced layer. It does not replace or change
the author-created examples and exercises stored in the main course catalogs.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import re
from dataclasses import dataclass
from pathlib import Path
from typing import Iterable

from pypdf import PdfReader


BASIC_SOURCE = {
    "id": "cke-basic-guide-2023",
    "level": "basic",
    "title": (
        "Informator o egzaminie maturalnym z matematyki od roku szkolnego "
        "2024/2025 - poziom podstawowy, Formuła 2023"
    ),
    "publisher": "Centralna Komisja Egzaminacyjna",
    "documentUrl": "https://bip.cke.gov.pl/attachments/download/10085",
    "documentSha256": "88A0EA8E2EE444506CCA5E89C860178E33B04F181650A36D9C9B4DC9BBE625B2",
    "verifiedOn": "2026-07-28",
    "firstExamplePage": 12,
    "lastExamplePage": 138,
    "exampleCount": 66,
}

EXTENDED_SOURCE = {
    "id": "cke-extended-guide-2023",
    "level": "extended",
    "title": (
        "Informator o egzaminie maturalnym z matematyki od roku szkolnego "
        "2024/2025 - poziom rozszerzony, Formuła 2023"
    ),
    "publisher": "Centralna Komisja Egzaminacyjna",
    "documentUrl": "https://bip.cke.gov.pl/attachments/download/10088",
    "documentSha256": "BD408CDC8877E04EC79AAC3177FAB304E6F66C6B5FA152D8D3436D4ACFB2BC6F",
    "verifiedOn": "2026-07-28",
    "firstExamplePage": 12,
    "lastExamplePage": 106,
    "exampleCount": 31,
}


# Descriptions cover figures that carry information not represented reliably by
# PDF text extraction. Page references let a reviewer compare every description
# directly with the pinned source document.
VISUAL_DESCRIPTIONS: dict[tuple[str, int], list[tuple[int, str]]] = {
    ("basic", 10): [(22, "Wykres rosnącej funkcji logarytmicznej w układzie współrzędnych, przechodzący przez zaznaczone punkty siatki.")],
    ("basic", 17): [(31, "Cztery osie liczbowe A-D przedstawiające proponowane zbiory rozwiązań nierówności za pomocą końców otwartych i domkniętych.")],
    ("basic", 19): [(35, "Parabola skierowana ramionami w górę oraz zaznaczony na osi zbiór rozwiązania nierówności.")],
    ("basic", 27): [(44, "Łamana będąca wykresem funkcji na przedziale od -5 do 8, z zaznaczonymi wierzchołkami w całkowitych punktach siatki.")],
    ("basic", 28): [(46, "Wykres funkcji przedziałami liniowej użyty do odczytania dziedziny, zbioru wartości i rozwiązań równania.")],
    ("basic", 29): [(48, "Wykres funkcji f oraz sześć wykresów A-F proponowanych jako wykresy przekształconych funkcji g i h.")],
    ("basic", 30): [(50, "Parabola skierowana ramionami w dół z zaznaczonym wierzchołkiem i przecięciami z osiami.")],
    ("basic", 31): [(52, "Parabola i jej przesunięty wykres pomocniczy w układzie współrzędnych.")],
    ("basic", 32): [(53, "Tor lotu piłki do kosza opisany fragmentem paraboli, z osiami odległości i wysokości oraz zaznaczonym koszem.")],
    ("basic", 33): [(58, "Cztery malejące wykresy A-D opisujące możliwe modele rozpadu promieniotwórczego."), (59, "Fotografia skamieniałości jaszczurki będącej kontekstem zadania o datowaniu radiowęglowym.")],
    ("basic", 38): [(67, "Dwa przylegające prostokąty o wspólnej wysokości, z podstawami długości 10 m i zmiennymi szerokościami x oraz y.")],
    ("basic", 41): [(73, "Okrąg ze średnicą AB długości 5, styczną BC i cięciwą AC, z punktem D oraz kątem 60 stopni."), (74, "Rysunki pomocnicze do rozwiązania z promieniami, cięciwami, styczną i zaznaczonymi kątami.")],
    ("basic", 43): [(78, "Okrąg o środku S z promieniami do punktów A, B i C oraz zaznaczonym kątem 50 stopni.")],
    ("basic", 44): [(79, "Trójkąt prostokątny ABC o przyprostokątnych 12 i 5 oraz przeciwprostokątnej 13, z medianami przecinającymi się w punkcie P.")],
    ("basic", 45): [(80, "Trójkąt ABC z punktem D na podstawie AB i punktem E na boku BC oraz odcinkami CD i DE.")],
    ("basic", 46): [(85, "Trzy trójkąty podobne ułożone wokół wspólnego trójkąta ABC; pola zewnętrznych trójkątów oznaczono P1, P2 i P3.")],
    ("basic", 47): [(89, "Prostokąt ABCD o bokach 4 i 2 z przekątną DB oraz odcinkiem CE tworzącym zaznaczone równe kąty.")],
    ("basic", 48): [(94, "Okrąg z cięciwą AD, promieniem SI prostopadłym do cięciwy oraz styczną w punkcie B przecinającą prostą AD w punkcie C.")],
    ("basic", 49): [(98, "Trójkąt ABC o bokach długości 4, 5 i 6 z zaznaczonym kątem przy wierzchołku A.")],
    ("basic", 50): [(100, "Trzy proste równoległe przecięte dwiema siecznymi; punkty A-I wyznaczają odpowiadające sobie odcinki.")],
    ("basic", 57): [(111, "Prostopadłościan ABCDEFGH z przekątną przestrzenną BH i przekątnymi ścian; cztery warianty zaznaczenia kąta alfa."), (112, "Prostopadłościan z zaznaczonymi krawędziami i przekątnymi użytymi w obliczeniu pola powierzchni.")],
    ("basic", 59): [(116, "Czapeczka w kształcie powierzchni bocznej stożka oraz odpowiadający jej wycinek koła i przekrój osiowy stożka."), (118, "Wycinek koła o promieniu l i kącie alfa oraz stożek o tworzącej l, promieniu r i wysokości H.")],
    ("basic", 60): [(121, "Graniastosłup prawidłowy czworokątny z zaznaczonym przekrojem i kątem 30 stopni przy przekątnej podstawy.")],
    ("basic", 62): [(123, "Sześć sześciopunktowych komórek alfabetu Braille'a zapisujących litery słowa matura."), (124, "Schemat przypisywania znaków Braille'a do sześciu ponumerowanych punktów."), (125, "Sześć ponumerowanych elementów przedstawionych w układzie dwóch kolumn i trzech wierszy.")],
    ("basic", 63): [(126, "Zestaw koszulek, spodni i par butów w podanych kolorach oraz ilustracja przykładowego stroju."), (128, "Ilustracje przypadków zliczania zestawów ubrań bez elementu niebieskiego oraz z jednym elementem niebieskim.")],
    ("basic", 64): [(130, "Cztery pola pozycyjne przedstawiające budowę liczby czterocyfrowej i dostępne cyfry."), (131, "Schematy rozmieszczenia cyfry 3 oraz zliczania pozostałych cyfr w liczbie czterocyfrowej."), (132, "Diagramy pozycyjne rozdzielające przypadki według liczby cyfr stojących po lewej stronie cyfry 3.")],
    ("basic", 65): [(133, "Fioletowy wykres słupkowy rozkładu miesięcznych zarobków pracowników firmy F.")],
    ("basic", 66): [(138, "Trójkątna tabela wyników dwóch losowań sześciennej kostki oraz drzewo wszystkich zdarzeń sprzyjających.")],
    ("extended", 3): [(18, "Wykres wielomianu stopnia trzeciego przecinający oś x w punkcie 1 i styczny do osi w drugim pierwiastku.")],
    ("extended", 9): [(31, "Reprodukcja przedstawiająca Syzyfa wtaczającego głaz, stanowiąca kontekst zadania optymalizacyjnego.")],
    ("extended", 10): [(33, "Schemat wyspy jako kwadratu z drogami łączącymi wierzchołki ze środkiem i wykres zależności czasu od położenia mostu.")],
    ("extended", 19): [(57, "Dwie prostopadłe drogi z punktami A, B, C i D oraz trasami dwóch zastępów harcerzy."), (62, "Dwa rysunki geometryczne pokazujące położenia zastępów w kolejnych godzinach.")],
    ("extended", 20): [(63, "Fotografia banknotów stanowiąca kontekst funkcji kosztu i przychodu przedsiębiorstwa.")],
    ("extended", 21): [(66, "Ostrosłup prawidłowy czworokątny z wysokością SO, punktem D na krawędzi i zaznaczonym kątem alfa."), (69, "Przekrój osiowy ostrosłupa i trójkąty pomocnicze użyte do wyprowadzenia zależności.")],
    ("extended", 25): [(81, "Sześcian ABCDEFGH o krawędzi a z punktem P będącym środkiem krawędzi CG.")],
    ("extended", 26): [(86, "Trapez prostokątny ABCD z okręgiem wpisanym, średnicą DP i odcinkami prowadzącymi do punktu B."), (87, "Cztery rysunki pomocnicze trapezu, okręgu i trójkątów podobnych.")],
    ("extended", 27): [(92, "Fotografia gór i model namiotu jako stożka z przekrojem osiowym, wysokością i promieniem podstawy."), (94, "Wycinek koła i trójkąt przekroju osiowego użyte do obliczenia najkrótszej drogi mrówki.")],
    ("extended", 29): [(98, "Fotografia szachownicy ilustrująca serię partii pana Nowaka z synem.")],
    ("extended", 31): [(102, "Schemat ulic w układzie prostokątnym z punktami A i B oraz fotografią parku stanowiącego przeszkodę."), (105, "Trzy schematy zliczania najkrótszych dróg na siatce ulic z ominięciem parku.")],
}


@dataclass(frozen=True)
class PageChunk:
    page: int
    text_start: int


def _sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for block in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(block)
    return digest.hexdigest().upper()


def _clean_page(text: str) -> str:
    lines: list[str] = []
    for line in text.splitlines():
        stripped = line.strip()
        if re.match(r"^\d+\s+Informator o egzaminie maturalnym", stripped):
            continue
        if re.match(r"^Przykładowe zadania z rozwiązaniami\s+\d+$", stripped):
            continue
        lines.append(line.rstrip())
    return "\n".join(lines).strip()


def _normalize_transcription(text: str) -> str:
    text = re.sub(r"\n<<<PAGE \d+>>>\n", "\n", text)
    text = text.replace("\u00ad", "")
    text = re.sub(r"[ \t]+\n", "\n", text)
    text = re.sub(r"\n{3,}", "\n\n", text)
    return text.strip()


def _source_pages(chunks: list[PageChunk], start: int, end: int) -> list[int]:
    pages = [chunk.page for chunk in chunks if chunk.text_start <= start]
    first_page = pages[-1]
    last_candidates = [chunk.page for chunk in chunks if chunk.text_start < end]
    return list(range(first_page, last_candidates[-1] + 1))


def _requirement_ids(text: str) -> list[str]:
    result: list[str] = []
    for details in re.finditer(r"Wymagani[ea] szczegółow[ea]", text):
        scoring = re.search(r"Zasady oceniania", text[details.end() :])
        if scoring is None:
            continue
        section = text[details.end() : details.end() + scoring.start()]
        headings = list(
            re.finditer(r"(?m)^\s*([IVX]+)\.\s+[^\n]*?Zdający:\s*$", section)
        )
        for index, heading in enumerate(headings):
            roman = heading.group(1).replace("IIII", "III")
            section_end = headings[index + 1].start() if index + 1 < len(headings) else len(section)
            body = section[heading.end() : section_end]
            for token in re.findall(r"(?m)^\s*(\d+R?|R)\)\s*", body):
                extended = token == "R" or token.endswith("R")
                number = "1" if token == "R" else token.rstrip("R")
                requirement_id = f"{roman}.{'E' if extended else 'B'}.{number}"
                if requirement_id not in result:
                    result.append(requirement_id)
    if not result and "XIII. Optymalizacja i rachunek różniczkowy" in text:
        result.append("XIII.B.1")
    return result


def _maximum_points(text: str, number: int) -> int:
    direct = re.search(
        rf"(?m)^Zadanie\s+{number}\.\s*\(0[–-](\d+)\)\s*$", text
    )
    if direct is not None:
        return int(direct.group(1))
    parts = re.findall(
        rf"(?m)^Zadanie\s+{number}\.\d+\.\s*\(0[–-](\d+)\)\s*$", text
    )
    if not parts:
        raise ValueError(f"Nie znaleziono punktacji zadania {number}.")
    return sum(int(value) for value in parts)


def _extract_examples(pdf_path: Path, source: dict[str, object]) -> list[dict[str, object]]:
    expected_hash = str(source["documentSha256"])
    actual_hash = _sha256(pdf_path)
    if actual_hash != expected_hash:
        raise ValueError(
            f"Nieprawidłowa suma SHA-256 pliku {pdf_path}: {actual_hash}, oczekiwano {expected_hash}."
        )

    reader = PdfReader(str(pdf_path))
    corpus_parts: list[str] = []
    chunks: list[PageChunk] = []
    offset = 0
    for page_number in range(int(source["firstExamplePage"]), int(source["lastExamplePage"]) + 1):
        marker = f"\n<<<PAGE {page_number}>>>\n"
        page_text = _clean_page(reader.pages[page_number - 1].extract_text() or "")
        corpus_parts.append(marker)
        offset += len(marker)
        chunks.append(PageChunk(page_number, offset))
        corpus_parts.append(page_text)
        offset += len(page_text)
    corpus = "".join(corpus_parts)

    starts: list[tuple[int, int]] = []
    expected_number = 1
    for match in re.finditer(
        r"(?m)^Zadanie\s+(\d+)\.\s*(?:\(0[–-]\d+\))?\s*$", corpus
    ):
        number = int(match.group(1))
        if number != expected_number:
            continue
        starts.append((number, match.start()))
        expected_number += 1
        if expected_number > int(source["exampleCount"]):
            break
    if len(starts) != int(source["exampleCount"]):
        raise ValueError(
            f"Odczytano {len(starts)} z {source['exampleCount']} zadań ze źródła {source['id']}."
        )

    examples: list[dict[str, object]] = []
    for index, (number, start) in enumerate(starts):
        end = starts[index + 1][1] if index + 1 < len(starts) else len(corpus)
        raw_text = corpus[start:end]
        transcription = _normalize_transcription(raw_text)
        requirements = _requirement_ids(transcription)
        if not requirements:
            raise ValueError(f"Zadanie {number} ze źródła {source['id']} nie ma mapowania wymagania.")
        pages = _source_pages(chunks, start, end)
        visual_references = [
            {"sourcePage": page, "alternativeText": description}
            for page, description in VISUAL_DESCRIPTIONS.get((str(source["level"]), number), [])
        ]
        examples.append(
            {
                "id": f"cke-{source['level']}-guide-task-{number:02}",
                "sourceId": source["id"],
                "level": source["level"],
                "officialNumber": str(number),
                "order": number,
                "maximumPoints": _maximum_points(transcription, number),
                "sourcePages": pages,
                "requirementIds": requirements,
                "visualReferences": visual_references,
                "transcription": transcription,
            }
        )
    return examples


def build_catalog(basic_pdf: Path, extended_pdf: Path) -> dict[str, object]:
    sources = [BASIC_SOURCE, EXTENDED_SOURCE]
    examples: list[dict[str, object]] = []
    for pdf_path, source in ((basic_pdf, BASIC_SOURCE), (extended_pdf, EXTENDED_SOURCE)):
        examples.extend(_extract_examples(pdf_path, source))
    return {
        "schemaVersion": 1,
        "sources": sources,
        "examples": examples,
    }


def write_catalog(catalog: dict[str, object], output_path: Path) -> None:
    output_path.parent.mkdir(parents=True, exist_ok=True)
    output_path.write_text(
        json.dumps(catalog, ensure_ascii=False, indent=2) + "\n",
        encoding="utf-8",
        newline="\n",
    )


def _parse_args(arguments: Iterable[str] | None = None) -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--basic-pdf", required=True, type=Path)
    parser.add_argument("--extended-pdf", required=True, type=Path)
    parser.add_argument(
        "--output",
        type=Path,
        default=Path("Content/official-course-examples.json"),
    )
    return parser.parse_args(arguments)


def main(arguments: Iterable[str] | None = None) -> int:
    args = _parse_args(arguments)
    catalog = build_catalog(args.basic_pdf, args.extended_pdf)
    write_catalog(catalog, args.output)
    print(
        f"Zapisano {len(catalog['examples'])} oficjalnych przykładów CKE do {args.output}."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
