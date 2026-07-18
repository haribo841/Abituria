#!/usr/bin/env python3
"""Build the controlled technical-documentation PDF for the Abituria commission package."""

from __future__ import annotations

from datetime import date
from pathlib import Path

from pypdf import PdfReader
from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER, TA_LEFT
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import cm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import (
    Flowable,
    HRFlowable,
    Image,
    KeepTogether,
    PageBreak,
    Paragraph,
    SimpleDocTemplate,
    Spacer,
    Table,
    TableStyle,
)


ROOT = Path(__file__).resolve().parents[1]
OUTPUT = ROOT / "output" / "pdf" / "Abituria-Technical-Documentation-0.9.0-beta.1.pdf"
SCREENSHOT = ROOT / "tests" / "Abituria.Tests" / "VisualBaselines" / "discussion-10-math-list-1280x820.png"
FONT_REGULAR = Path(r"C:\Windows\Fonts\arial.ttf")
FONT_BOLD = Path(r"C:\Windows\Fonts\arialbd.ttf")


def register_fonts() -> None:
    pdfmetrics.registerFont(TTFont("Abituria", str(FONT_REGULAR)))
    pdfmetrics.registerFont(TTFont("Abituria-Bold", str(FONT_BOLD)))


def styles() -> dict[str, ParagraphStyle]:
    sample = getSampleStyleSheet()
    return {
        "Title": ParagraphStyle(
            "AbituriaTitle",
            parent=sample["Title"],
            fontName="Abituria-Bold",
            fontSize=25,
            leading=31,
            textColor=colors.HexColor("#12355B"),
            alignment=TA_CENTER,
            spaceAfter=14,
        ),
        "Subtitle": ParagraphStyle(
            "AbituriaSubtitle",
            parent=sample["Normal"],
            fontName="Abituria",
            fontSize=12,
            leading=17,
            alignment=TA_CENTER,
            textColor=colors.HexColor("#334155"),
        ),
        "Heading": ParagraphStyle(
            "AbituriaHeading",
            parent=sample["Heading1"],
            fontName="Abituria-Bold",
            fontSize=16,
            leading=21,
            textColor=colors.HexColor("#12355B"),
            spaceBefore=12,
            spaceAfter=7,
        ),
        "Subheading": ParagraphStyle(
            "AbituriaSubheading",
            parent=sample["Heading2"],
            fontName="Abituria-Bold",
            fontSize=12,
            leading=16,
            textColor=colors.HexColor("#1D4ED8"),
            spaceBefore=9,
            spaceAfter=5,
        ),
        "Body": ParagraphStyle(
            "AbituriaBody",
            parent=sample["BodyText"],
            fontName="Abituria",
            fontSize=9.5,
            leading=14,
            alignment=TA_LEFT,
            spaceAfter=7,
        ),
        "Small": ParagraphStyle(
            "AbituriaSmall",
            parent=sample["BodyText"],
            fontName="Abituria",
            fontSize=8,
            leading=10.5,
            textColor=colors.HexColor("#475569"),
        ),
        "Table": ParagraphStyle(
            "AbituriaTable",
            parent=sample["BodyText"],
            fontName="Abituria",
            fontSize=7.6,
            leading=10,
        ),
        "TableBold": ParagraphStyle(
            "AbituriaTableBold",
            parent=sample["BodyText"],
            fontName="Abituria-Bold",
            fontSize=7.6,
            leading=10,
            textColor=colors.white,
        ),
        "Caption": ParagraphStyle(
            "AbituriaCaption",
            parent=sample["BodyText"],
            fontName="Abituria",
            fontSize=7.8,
            leading=10,
            alignment=TA_CENTER,
            textColor=colors.HexColor("#475569"),
            spaceAfter=8,
        ),
    }


def p(text: str, style: ParagraphStyle) -> Paragraph:
    return Paragraph(text, style)


def table(rows: list[list[str]], widths: list[float], st: dict[str, ParagraphStyle]) -> Table:
    rendered = [[p(cell, st["TableBold"] if row_index == 0 else st["Table"]) for cell in row] for row_index, row in enumerate(rows)]
    result = Table(rendered, colWidths=widths, repeatRows=1, hAlign="LEFT")
    result.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (-1, 0), colors.HexColor("#12355B")),
                ("TEXTCOLOR", (0, 0), (-1, 0), colors.white),
                ("GRID", (0, 0), (-1, -1), 0.25, colors.HexColor("#CBD5E1")),
                ("VALIGN", (0, 0), (-1, -1), "TOP"),
                ("LEFTPADDING", (0, 0), (-1, -1), 5),
                ("RIGHTPADDING", (0, 0), (-1, -1), 5),
                ("TOPPADDING", (0, 0), (-1, -1), 4),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 4),
                ("ROWBACKGROUNDS", (0, 1), (-1, -1), [colors.white, colors.HexColor("#F8FAFC")]),
            ]
        )
    )
    return result


class ComponentDiagram(Flowable):
    """A small, self-contained architectural component diagram."""

    def __init__(self) -> None:
        super().__init__()
        self.width = 17.2 * cm
        self.height = 10.3 * cm

    def wrap(self, available_width: float, available_height: float) -> tuple[float, float]:
        return self.width, self.height

    def draw_box(self, canvas, x: float, y: float, width: float, height: float, title: str, detail: str) -> None:
        canvas.setStrokeColor(colors.HexColor("#2563EB"))
        canvas.setFillColor(colors.HexColor("#EFF6FF"))
        canvas.roundRect(x, y, width, height, 6, stroke=1, fill=1)
        canvas.setFillColor(colors.HexColor("#12355B"))
        canvas.setFont("Abituria-Bold", 8)
        canvas.drawCentredString(x + width / 2, y + height - 14, title)
        canvas.setFillColor(colors.HexColor("#334155"))
        canvas.setFont("Abituria", 7)
        for index, line in enumerate(detail.split("\n")):
            canvas.drawCentredString(x + width / 2, y + height - 27 - index * 9, line)

    def arrow(self, canvas, start_x: float, start_y: float, end_x: float, end_y: float) -> None:
        canvas.setStrokeColor(colors.HexColor("#64748B"))
        canvas.setLineWidth(1)
        canvas.line(start_x, start_y, end_x, end_y)
        canvas.setFillColor(colors.HexColor("#64748B"))
        canvas.circle(end_x, end_y, 2.4, stroke=0, fill=1)

    def draw(self) -> None:
        canvas = self.canv
        canvas.setFont("Abituria-Bold", 10)
        canvas.setFillColor(colors.HexColor("#12355B"))
        canvas.drawString(0, self.height - 12, "Diagram komponentów aktualnej implementacji")

        boxes = {
            "ui": (0.2 * cm, 5.8 * cm, 3.5 * cm, 1.35 * cm, "UI", "MainWindow\nViews + UiFactory"),
            "vm": (4.4 * cm, 5.8 * cm, 3.5 * cm, 1.35 * cm, "Stan i nawigacja", "AppViewModel\nwybrany kontekst"),
            "services": (8.6 * cm, 5.45 * cm, 3.9 * cm, 2.0 * cm, "Usługi", "ContentRepository\nAccountService, kalkulatory\nExerciseRandomizer"),
            "content": (13.3 * cm, 6.1 * cm, 3.6 * cm, 1.1 * cm, "Treści", "JSON, fonty, obrazy"),
            "sqlite": (13.3 * cm, 4.2 * cm, 3.6 * cm, 1.1 * cm, "Dane lokalne", "SQLite + EF Core"),
            "tests": (4.4 * cm, 2.3 * cm, 8.1 * cm, 1.3 * cm, "Walidacja", "xUnit, Avalonia Headless, smoke test wydania, CI i SonarQube"),
        }
        for box in boxes.values():
            self.draw_box(canvas, *box)
        self.arrow(canvas, 3.7 * cm, 6.47 * cm, 4.4 * cm, 6.47 * cm)
        self.arrow(canvas, 7.9 * cm, 6.47 * cm, 8.6 * cm, 6.47 * cm)
        self.arrow(canvas, 12.5 * cm, 6.8 * cm, 13.3 * cm, 6.8 * cm)
        self.arrow(canvas, 12.5 * cm, 5.05 * cm, 13.3 * cm, 4.75 * cm)
        self.arrow(canvas, 8.0 * cm, 5.45 * cm, 8.0 * cm, 3.6 * cm)


def footer(canvas, doc) -> None:
    canvas.saveState()
    canvas.setStrokeColor(colors.HexColor("#CBD5E1"))
    canvas.line(doc.leftMargin, 1.35 * cm, A4[0] - doc.rightMargin, 1.35 * cm)
    canvas.setFont("Abituria", 7.5)
    canvas.setFillColor(colors.HexColor("#475569"))
    canvas.drawString(doc.leftMargin, 0.92 * cm, "Abituria - dokumentacja techniczna dla komisji")
    canvas.drawRightString(A4[0] - doc.rightMargin, 0.92 * cm, f"strona {doc.page}")
    canvas.restoreState()


def build_pdf() -> None:
    if not FONT_REGULAR.exists() or not FONT_BOLD.exists():
        raise FileNotFoundError("Nie znaleziono wymaganych czcionek Arial w katalogu Windows Fonts.")
    if not SCREENSHOT.exists():
        raise FileNotFoundError(f"Nie znaleziono zrzutu testu wizualnego: {SCREENSHOT}")

    register_fonts()
    st = styles()
    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    document = SimpleDocTemplate(
        str(OUTPUT),
        pagesize=A4,
        leftMargin=1.6 * cm,
        rightMargin=1.6 * cm,
        topMargin=1.55 * cm,
        bottomMargin=1.8 * cm,
        title="Abituria - dokumentacja techniczna",
        author="Adam Kubiś",
        subject="Pakiet dokumentacji technicznej dla komisji",
    )

    story: list[Flowable] = []
    story.extend(
        [
            Spacer(1, 3.0 * cm),
            p("Abituria", st["Title"]),
            p("Dokumentacja techniczna dla komisji", st["Subtitle"]),
            Spacer(1, 0.6 * cm),
            HRFlowable(width="65%", color=colors.HexColor("#2563EB"), thickness=1.2),
            Spacer(1, 0.6 * cm),
            p("Wersja: <b>0.9.0-beta.1</b>", st["Subtitle"]),
            p(f"Data wygenerowania: <b>{date.today().isoformat()}</b>", st["Subtitle"]),
            p("Commit źródłowy: <b>do wpisania przy przekazaniu lub tagowaniu wydania</b>", st["Subtitle"]),
            p("Kod źródłowy: <b>github.com/haribo841/Abituria</b>", st["Subtitle"]),
            Spacer(1, 2.0 * cm),
            p(
                "Status dokumentu: pakiet techniczny przygotowany do odbioru. Nie jest to potwierdzenie "
                "publicznego wydania ani decyzja komisji. Te zdarzenia wymagają osobnych, udokumentowanych bram.",
                st["Body"],
            ),
            PageBreak(),
        ]
    )

    story.extend(
        [
            p("1. Cel i zakres systemu", st["Heading"]),
            p(
                "Abituria jest lokalną aplikacją desktopową wspierającą naukę matematyki na poziomie szkoły "
                "średniej i przygotowanie do matury. Łączy materiały teoretyczne, zadania, stopniowane "
                "podpowiedzi, sprawdzanie odpowiedzi oraz dwa kalkulatory. Podstawowe użycie nie wymaga "
                "połączenia z Internetem.",
                st["Body"],
            ),
            p("Zakres aktualnej implementacji", st["Subheading"]),
            table(
                [
                    ["Obszar", "Zakres"],
                    ["Nauka", "18 tablic, 7 dostępnych działów, przykłady, zadania, podpowiedzi i odpowiedzi."],
                    ["Zadania", "35 zadań z podziałem na tematy, zapis postępu, brudnopis oraz losowanie z całej puli lub tematu."],
                    ["Kalkulatory", "Kalkulator funkcji kwadratowej oraz parser wyrażeń z potęgami, pierwiastkami, Ans i historią."],
                    ["Dane", "Profile gościa i konta lokalne, SQLite, hasła chronione PBKDF2-HMAC-SHA256."],
                    ["Wydanie", "Paczki self-contained x64 przygotowane dla Windows, Ubuntu i macOS Intel, z kontrolowanym smoke testem."],
                ],
                [3.2 * cm, 14.0 * cm],
                st,
            ),
            p("Poza zakresem beta", st["Subheading"]),
            p(
                "Nie są zaimplementowane: pełny generator wykresów, kalkulator trygonometryczny, konta "
                "sieciowe, synchronizacja, automatyczne aktualizacje, podpisywanie kodu, instalatory natywne "
                "oraz dystrybucja dla Apple Silicon.",
                st["Body"],
            ),
            p("2. Architektura i technologie", st["Heading"]),
            p(
                "Aplikacja jest napisana w C# dla .NET 10 i wykorzystuje AvaloniaUI 12. Jeden MainWindow "
                "udostępnia shell aplikacji, a AppViewModel utrzymuje stan sesji i kontekst nawigacji. Usługi "
                "są rejestrowane przez Microsoft.Extensions.DependencyInjection. Treści są zasobami JSON, "
                "dane użytkownika są przechowywane lokalnie w SQLite przez Entity Framework Core.",
                st["Body"],
            ),
            ComponentDiagram(),
            p("Rysunek 1. Diagram komponentów aktualnej implementacji.", st["Caption"]),
            PageBreak(),
        ]
    )

    story.extend(
        [
            p("3. Główne moduły i nawigacja", st["Heading"]),
            table(
                [
                    ["Moduł", "Odpowiedzialność", "Kluczowe elementy"],
                    ["Shell i nawigacja", "Jedno okno, przełączanie widoków i kontekst zadania.", "MainWindow, AppViewModel"],
                    ["Treści", "Ładowanie tablic, działów, zadań i tekstów interfejsu.", "ContentRepository, RichContentView"],
                    ["Zadania", "Lista, tematy, podpowiedzi, odpowiedzi, postęp i losowanie.", "ExamOverviewView, ExerciseView, ExerciseRandomizer"],
                    ["Kalkulatory", "Parser wyrażeń i analiza funkcji kwadratowej.", "ExpressionCalculator, CalculatorSession, QuadraticSolver"],
                    ["Konta", "Profile, rejestracja, hasła, odzyskiwanie i zapis postępu.", "AccountService, PasswordHasher, AppDbContext"],
                    ["Wydanie", "Smoke test, wersja, paczki, SBOM i walidacja archiwów.", "ReleaseSmokeTest, tools/release"],
                ],
                [3.0 * cm, 7.2 * cm, 7.0 * cm],
                st,
            ),
            p("Nawigacja", st["Subheading"]),
            p(
                "Po zalogowaniu użytkownik korzysta z górnej nawigacji do ekranu Start, tablic, zadań, "
                "działów, kalkulatora, planu rozwoju, profilu i informacji o programie. Otwieranie zadania "
                "zapamiętuje kontekst pełnego arkusza albo tematu. Losowanie zadań używa tego samego kontekstu, "
                "więc przejście do poprzedniego lub następnego zadania nie opuszcza wybranej puli.",
                st["Body"],
            ),
            p("4. Wymagania systemowe i uruchomienie", st["Heading"]),
            table(
                [
                    ["Platforma", "Deklarowany wariant", "Wymagania dodatkowe"],
                    ["Windows", "Windows 11 x64", "Paczka portable self-contained, bez instalacji .NET."],
                    ["Ubuntu", "Ubuntu 24.04 x64", "libx11-6, libice6, libsm6 i libfontconfig1."],
                    ["macOS", "macOS 15 Intel x64", "Paczka portable, brak wsparcia Apple Silicon."],
                ],
                [3.0 * cm, 4.2 * cm, 10.0 * cm],
                st,
            ),
            p(
                "Użytkownik pobiera jedną paczkę zgodną z systemem, porównuje SHA-256 z manifestem wydania, "
                "rozpakowuje ją do nowego katalogu i uruchamia plik wykonywalny. Dane użytkownika są poza "
                "katalogiem programu. Pełną instrukcję instalacji, aktualizacji i odinstalowania zawiera "
                "INSTALLATION.md.",
                st["Body"],
            ),
            PageBreak(),
        ]
    )

    story.extend(
        [
            p("5. Weryfikacja jakości i testy", st["Heading"]),
            p(
                "Zestaw xUnit obejmuje testy jednostkowe, integracyjne, regresyjne, wizualne, headless UI, "
                "systemowe i kontraktowe wydania. Automatyczna brama wykonuje restore z lockfile, build Release, "
                "testy, formatowanie, audyt NuGet, walidację pochodzenia treści oraz analizę SonarQube.",
                st["Body"],
            ),
            table(
                [
                    ["Rodzaj", "Przykładowy dowód", "Cel"],
                    ["Funkcjonalne", "AccountServiceTests, ExerciseAndRoutingCoverageTests", "Konta, zadania, podpowiedzi i postęp."],
                    ["Regresyjne", "ExpressionCalculatorRobustnessTests, Issue35MathChaptersRegressionTests", "Błędy kalkulatora, kompletność treści i zachowanie UI."],
                    ["Wizualne", "Discussion10VisualRegressionTests", "Układ matematyki przy 960x640 i 1280x820."],
                    ["Wydanie", "ReleaseRuntimeTests, Invoke-ReleaseSmokeTest.ps1", "Wersja, izolowana baza i rzeczywisty plik wykonywalny paczki."],
                    ["Wydajność", "PerformanceMemoryAndLoadTests", "Czas, alokacje, pamięć zachowana i zachowanie pod obciążeniem."],
                ],
                [3.0 * cm, 7.0 * cm, 7.2 * cm],
                st,
            ),
            p("Wyniki niefunkcjonalne z wykonania Release 18 lipca 2026 r.", st["Subheading"]),
            table(
                [
                    ["Scenariusz", "Wynik", "Próg"],
                    ["30 000 obliczeń kalkulatora", "101,9 ms; 2 264 B na obliczenie", "15 s; 12 KiB na obliczenie"],
                    ["40 000 obliczeń równoległych", "149,9 ms; 0 błędów", "20 s; 0 błędów"],
                    ["20 przeładowań treści", "17,0 ms; 4 941 208 B; 227 096 B po GC", "15 s; 64 MiB; 32 MiB po GC"],
                    ["SQLite: 5 000 wpisów, 3 odczyty", "56,4 ms; 5 211 488 B; baza 885 616 B", "15 s; 64 MiB; baza 16 MiB"],
                ],
                [4.0 * cm, 7.0 * cm, 6.2 * cm],
                st,
            ),
            p("Zrzut z aktualnego testu wizualnego", st["Subheading"]),
        ]
    )
    screenshot = Image(str(SCREENSHOT), width=16.2 * cm, height=10.37 * cm)
    story.append(screenshot)
    story.append(p("Rysunek 2. Renderowana lista matematyczna używana jako obraz wzorcowy w teście wizualnym 1280x820.", st["Caption"]))
    story.append(PageBreak())

    story.extend(
        [
            p("6. Ograniczenia, licencja i status wydania", st["Heading"]),
            p(
                "Kod projektu jest objęty licencją MIT. Licencje bibliotek są opisane w THIRD-PARTY-NOTICES.md, "
                "a autor aktualnej implementacji jest wskazany w AUTHORS.md. Aplikacja nie ma automatycznych "
                "aktualizacji, podpisu kodu ani pełnego audytu dostępności WCAG.",
                st["Body"],
            ),
            p("Bramy publicznego wydania", st["Subheading"]),
            table(
                [
                    ["Brama", "Stan", "Znaczenie"],
                    ["Build, testy, format i audyt", "PASS: 4fdecd2", "Build i SonarCloud zaliczone 18 lipca 2026 r."],
                    ["Instalacja niezależna i użyteczność", "CI PASS / ręczna pending", "Natywne runnery Windows, Ubuntu i macOS zaliczone; wymagane wpisy i decyzja osoby."],
                    ["Pochodzenie zasobów", "zablokowana", "Manifest wskazuje materiały CKE oraz odziedziczone grafiki bez potwierdzonej podstawy redystrybucji."],
                    ["GitHub Release", "nieuruchomiona", "Może powstać dopiero po pozytywnym wyniku pochodzenia, tagu i pełnym workflow."],
                ],
                [4.1 * cm, 3.2 * cm, 9.9 * cm],
                st,
            ),
            p(
                "Nie można uznać lokalnego artefaktu za publiczne wydanie. Aktualna blokada prawna jest jawnie "
                "opisana w Content/provenance.json i CONTENT_PROVENANCE.md. Wymaga albo weryfikowalnych licencji, "
                "albo pełnego zastąpienia wskazanych treści i grafik własnymi zasobami.",
                st["Body"],
            ),
            PageBreak(),
            p("7. Materiały przekazywane komisji", st["Heading"]),
            p(
                "Indeks dokumentów źródłowych znajduje się w COMMISSION_PACKAGE.md. Obejmuje wymagania, architekturę, "
                "instrukcję instalacji, podręcznik użytkownika, zależności, raport testów, protokoły użyteczności "
                "i odbioru, ograniczenia, licencję, autorstwo oraz historię zmian.",
                st["Body"],
            ),
            p("8. Potwierdzenie przekazania i odbioru", st["Heading"]),
            p(
                "Poniższe pola są celowo puste. Ich wypełnienie wymaga rzeczywistego działania osoby przekazującej "
                "i osoby odbierającej. Nie są automatycznie wypełniane przez narzędzie budujące dokument.",
                st["Body"],
            ),
            Spacer(1, 0.6 * cm),
            table(
                [
                    ["Rola", "Imię i nazwisko", "Data", "Podpis lub identyfikator zatwierdzenia"],
                    ["Przekazujący dokumentację", "", "", ""],
                    ["Osoba odpowiedzialna za odbiór", "", "", ""],
                    ["Komisja / opiekun", "", "", ""],
                ],
                [4.1 * cm, 4.1 * cm, 3.0 * cm, 6.0 * cm],
                st,
            ),
        ]
    )

    document.build(story, onFirstPage=footer, onLaterPages=footer)
    reader = PdfReader(str(OUTPUT))
    if len(reader.pages) < 6:
        raise RuntimeError("Wygenerowany PDF ma zbyt mało stron, aby zawierać wymagany pakiet.")
    first_page = reader.pages[0].extract_text() or ""
    if "Abituria" not in first_page:
        raise RuntimeError("Wygenerowany PDF nie zawiera oczekiwanego tytułu.")
    print(f"Wygenerowano {OUTPUT} ({len(reader.pages)} stron).")


if __name__ == "__main__":
    build_pdf()
