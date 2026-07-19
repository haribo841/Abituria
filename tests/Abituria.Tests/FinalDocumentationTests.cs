namespace Abituria.Tests;

public sealed class FinalDocumentationTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Commission_package_contains_the_required_current_documents()
    {
        var required = new Dictionary<string, string[]>
        {
            ["docs/TESTING.md"] = ["## Cel i zakres", "## Wyniki wydajności, pamięci i obciążenia"],
            ["docs/USABILITY_TEST_PROTOCOL.md"] = ["## Scenariusze", "## Arkusz wyników"],
            ["docs/USABILITY_TEST_RESULTS.md"] = ["## Retrospektywne poświadczenie właściciela", "## Runda przed III odbiorem", "## Runda przed IV odbiorem", "## Poprawki wynikające z badania", "## Retest poprawek"],
            ["docs/ACCEPTANCE_PROTOCOL.md"] = ["## Podstawa retrospektywnego odbioru", "## Automatyczny test instalacji na niezależnych komputerach", "## Ręczny test instalacji na niezależnych komputerach", "## Decyzja odbiorowa"],
            ["docs/DELIVERY_PROTOCOL.md"] = ["## Dopuszczalne warianty przekazania", "## Manifest przekazywanego pakietu", "## Zgoda prowadzącego lub odbiorcy", "## Decyzja końcowa i dowody"],
            ["docs/DEFENSE_PROTOCOL.md"] = ["## Identyfikacja wydarzenia", "## Komisja", "## Przebieg obrony", "## Materiały i dowody", "## Kryteria gotowości do obrony", "## Decyzja komisji", "## Checklista zamknięcia Issue #44"],
            ["docs/EVALUATION_PROTOCOL.md"] = ["## Cel i zakres protokołu", "## Źródła dowodowe", "## Dowód prezentacji i demonstracji", "## Ocena siedmiu obszarów", "## Warunki uznania projektu za zaakceptowany", "## Warunki uzyskania oceny bardzo dobrej", "## Praca zespołu i autorstwo", "## Decyzja formalna dla Issue #45", "## Checklista formalnego zamknięcia", "## Treść komentarza zamykającego"],
            ["docs/COMMISSION_PACKAGE.md"] = ["## Zawartość techniczna", "## PDF"],
            ["docs/acceptance/README.md"] = ["## Podstawa historycznego odbioru", "## Ocena kompletności Issue #43", "## Publiczne wydanie obecnej migracji"]
        };

        foreach (var (relativePath, headings) in required)
        {
            var path = Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(path), $"Brak dokumentu: {relativePath}");
            var text = File.ReadAllText(path);
            Assert.All(headings, heading => Assert.Contains(heading, text, StringComparison.Ordinal));
        }
    }

    [Fact]
    public void Issue_43_has_four_separate_increment_protocols()
    {
        var protocols = Enumerable.Range(1, 4)
            .Select(number => $"docs/acceptance/INCREMENT_{number}_PROTOCOL.md")
            .ToArray();
        var commonHeadings = new[]
        {
            "## Identyfikacja i daty",
            "## Zakres planowany",
            "## Rekonstrukcja techniczna zakresu",
            "## Wyniki testów i dowody",
            "## Zachowane ograniczenia dokumentacyjne",
            "## Uwagi",
            "## Decyzje"
        };

        foreach (var relativePath in protocols)
        {
            var path = Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(path), $"Brak protokołu: {relativePath}");
            var text = File.ReadAllText(path);
            Assert.All(commonHeadings, heading => Assert.Contains(heading, text, StringComparison.Ordinal));
        }

        Assert.Contains("## Zakres następnego przyrostu", File.ReadAllText(Absolute(protocols[0])), StringComparison.Ordinal);
        Assert.Contains("## Zakres następnego przyrostu", File.ReadAllText(Absolute(protocols[1])), StringComparison.Ordinal);
        Assert.Contains("## Zakres następnego przyrostu", File.ReadAllText(Absolute(protocols[2])), StringComparison.Ordinal);
        Assert.Contains("## Wniosek dla Issue #43", File.ReadAllText(Absolute(protocols[3])), StringComparison.Ordinal);
    }

    [Fact]
    public void Issue_43_manual_and_legal_gates_are_discoverable_and_not_conflated_with_ci()
    {
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));
        var readme = File.ReadAllText(Absolute("README.md"));
        var commissionPackage = File.ReadAllText(Absolute("docs/COMMISSION_PACKAGE.md"));
        var acceptance = File.ReadAllText(Absolute("docs/ACCEPTANCE_PROTOCOL.md"));
        var usability = File.ReadAllText(Absolute("docs/USABILITY_TEST_RESULTS.md"));
        var delivery = File.ReadAllText(Absolute("docs/DELIVERY_PROTOCOL.md"));
        var handoffScript = File.ReadAllText(Absolute("tools/New-CommissionHandoffPackage.ps1"));

        foreach (var fileName in new[]
                 {
                     "INCREMENT_1_PROTOCOL.md",
                     "INCREMENT_2_PROTOCOL.md",
                     "INCREMENT_3_PROTOCOL.md",
                     "INCREMENT_4_PROTOCOL.md"
                 })
        {
            Assert.Contains(fileName, toc, StringComparison.Ordinal);
            Assert.Contains(fileName, commissionPackage, StringComparison.Ordinal);
        }

        Assert.Contains("docs/acceptance/README.md", readme, StringComparison.Ordinal);
        Assert.Contains("## Checklista zamknięcia Issue #43", acceptance, StringComparison.Ordinal);
        Assert.Contains("## Uczestnicy i zgody", usability, StringComparison.Ordinal);
        Assert.Contains("## Decyzje po rundach", usability, StringComparison.Ordinal);
        Assert.Contains("-RequireReleaseEligible", delivery, StringComparison.Ordinal);
        Assert.Contains("nie jest uruchamialny", delivery, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("commission-documentation-candidate", handoffScript, StringComparison.Ordinal);
        Assert.Contains("isRunnable = $false", handoffScript, StringComparison.Ordinal);
        Assert.Contains("cke-2021-correction-exam", handoffScript, StringComparison.Ordinal);
        Assert.Contains("inherited-mathematics-images", handoffScript, StringComparison.Ordinal);
        Assert.Contains("inherited-application-images", handoffScript, StringComparison.Ordinal);
    }

    [Fact]
    public void Issue_43_retrospective_evidence_is_honest_and_publication_remains_separate()
    {
        var usabilityProtocol = File.ReadAllText(Absolute("docs/USABILITY_TEST_PROTOCOL.md"));
        var usabilityResults = File.ReadAllText(Absolute("docs/USABILITY_TEST_RESULTS.md"));
        var acceptance = File.ReadAllText(Absolute("docs/ACCEPTANCE_PROTOCOL.md"));
        var delivery = File.ReadAllText(Absolute("docs/DELIVERY_PROTOCOL.md"));
        var requirements = File.ReadAllText(Absolute("docs/REQUIREMENTS.md"));

        Assert.Contains("historyczne testy użyteczności z uczestnikami zakończyły się powodzeniem", usabilityProtocol, StringComparison.Ordinal);
        Assert.Contains("nie ustala ani nie odtwarza przez domysł", usabilityResults, StringComparison.Ordinal);
        Assert.Contains("Nie są przypisywane uczestnikom historycznych badań", usabilityResults, StringComparison.Ordinal);
        Assert.Contains("początek lutego 2022 r.; dokładny dzień nie został zachowany", acceptance, StringComparison.Ordinal);
        Assert.Contains("ACCEPTED - READY TO CLOSE", acceptance, StringComparison.Ordinal);
        Assert.Contains("Właściciel projektu poświadczył", delivery, StringComparison.Ordinal);
        Assert.Contains("Nie utworzono tagu ani GitHub Release", delivery, StringComparison.Ordinal);
        Assert.Contains("Publiczny GitHub Release `0.9.0-beta.1` nie istnieje", requirements, StringComparison.Ordinal);

        var provenance = File.ReadAllText(Absolute("Content/provenance.json"));
        Assert.Contains("\"releaseEligible\": true", provenance, StringComparison.Ordinal);
        Assert.DoesNotContain("\"distributionStatus\": \"blocked\"", provenance, StringComparison.Ordinal);
        Assert.True(File.Exists(Absolute("docs/ASSET_RIGHTS_DECLARATION.md")), "Brak deklaracji praw do zasobów.");
    }

    [Fact]
    public void Issue_44_public_defense_records_the_attested_event_and_result()
    {
        var defense = File.ReadAllText(Absolute("docs/DEFENSE_PROTOCOL.md"));

        Assert.Contains("17 stycznia 2022 r.", defense, StringComparison.Ordinal);
        Assert.Contains("| Przewodniczący | dr Paweł M. Owsianny |", defense, StringComparison.Ordinal);
        Assert.Contains("| Recenzent | prof. UAM dr hab. Jerzy Szymański |", defense, StringComparison.Ordinal);
        Assert.Contains("| Promotor | dr Tomasz Piłka |", defense, StringComparison.Ordinal);
        Assert.Contains("Min_5DBHHnQ", defense, StringComparison.Ordinal);
        Assert.Contains("2:10:03", defense, StringComparison.Ordinal);
        Assert.Contains("unlisted", defense, StringComparison.Ordinal);
        Assert.Contains("2:09:18", defense, StringComparison.Ordinal);
        Assert.Contains("Transmisja kończy się przed ogłoszeniem formalnej decyzji komisji", defense, StringComparison.Ordinal);
        Assert.Contains("pokazano działającą aplikację", defense, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("pytania i odpowiedzi", defense, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("| Decyzja | pozytywna |", defense, StringComparison.Ordinal);
        Assert.Contains("| Wynik | bardzo dobry |", defense, StringComparison.Ordinal);
        Assert.Contains("ACCEPTED - M7 ACHIEVED - READY TO CLOSE AS COMPLETED", defense, StringComparison.Ordinal);
        Assert.DoesNotContain("- [ ]", defense, StringComparison.Ordinal);
    }

    [Fact]
    public void Issue_44_defense_protocol_is_discoverable_and_packaged()
    {
        var readme = File.ReadAllText(Absolute("README.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));
        var commissionPackage = File.ReadAllText(Absolute("docs/COMMISSION_PACKAGE.md"));
        var acceptance = File.ReadAllText(Absolute("docs/ACCEPTANCE_PROTOCOL.md"));
        var requirements = File.ReadAllText(Absolute("docs/REQUIREMENTS.md"));
        var handoffScript = File.ReadAllText(Absolute("tools/New-CommissionHandoffPackage.ps1"));

        Assert.Contains("docs/DEFENSE_PROTOCOL.md", readme, StringComparison.Ordinal);
        Assert.Contains("href: DEFENSE_PROTOCOL.md", toc, StringComparison.Ordinal);
        Assert.Contains("DEFENSE_PROTOCOL.md", commissionPackage, StringComparison.Ordinal);
        Assert.Contains("DEFENSE_PROTOCOL.md", acceptance, StringComparison.Ordinal);
        Assert.Contains("## Status issue #44", requirements, StringComparison.Ordinal);
        Assert.Contains("docs/DEFENSE_PROTOCOL.md", handoffScript, StringComparison.Ordinal);
    }

    [Fact]
    public void Issue_45_evaluation_records_historical_acceptance_without_claiming_current_release()
    {
        var evaluation = File.ReadAllText(Absolute("docs/EVALUATION_PROTOCOL.md"));

        Assert.Contains("Issue #45 - Kryteria akceptacji", evaluation, StringComparison.Ordinal);
        Assert.Contains("17 stycznia 2022 r.", evaluation, StringComparison.Ordinal);
        Assert.Contains("Min_5DBHHnQ", evaluation, StringComparison.Ordinal);
        Assert.Contains("unlisted", evaluation, StringComparison.Ordinal);
        Assert.Contains("2:09:18", evaluation, StringComparison.Ordinal);
        Assert.Contains("v1.0.0", evaluation, StringComparison.Ordinal);
        Assert.Contains("v1.0.1", evaluation, StringComparison.Ordinal);
        Assert.Contains("Pre-release.zip", evaluation, StringComparison.Ordinal);
        Assert.Contains("837C38D0EB8A53E56B51598B07FE8087C6E0B7E85A398394AFFF04A8353A0DC1", evaluation, StringComparison.Ordinal);
        Assert.Contains("E88F327B172B1CD78A0F7E9F646378D6032267DA16464218051D0F030E87D6DE", evaluation, StringComparison.Ordinal);
        Assert.Contains("nie mają wspólnego przodka", evaluation, StringComparison.Ordinal);
        Assert.Contains("Nie zawiera ogłoszenia formalnej decyzji ani oceny bardzo dobrej", evaluation, StringComparison.Ordinal);
        Assert.Contains("projekt i obrona uzyskały wynik bardzo dobry", evaluation, StringComparison.Ordinal);
        Assert.Contains("ACCEPTED - HISTORICAL CRITERIA SATISFIED - READY TO CLOSE AS COMPLETED", evaluation, StringComparison.Ordinal);
        Assert.Contains("TECHNICALLY ACCEPTED - PUBLIC RELEASE PENDING", evaluation, StringComparison.Ordinal);
        Assert.DoesNotContain("- [ ]", evaluation, StringComparison.Ordinal);
    }

    [Fact]
    public void Issue_45_evaluation_protocol_is_discoverable_and_packaged()
    {
        var readme = File.ReadAllText(Absolute("README.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));
        var commissionPackage = File.ReadAllText(Absolute("docs/COMMISSION_PACKAGE.md"));
        var acceptance = File.ReadAllText(Absolute("docs/ACCEPTANCE_PROTOCOL.md"));
        var requirements = File.ReadAllText(Absolute("docs/REQUIREMENTS.md"));
        var handoffScript = File.ReadAllText(Absolute("tools/New-CommissionHandoffPackage.ps1"));
        var pdfGenerator = File.ReadAllText(Absolute("tools/New-CommissionPdf.py"));

        Assert.Contains("docs/EVALUATION_PROTOCOL.md", readme, StringComparison.Ordinal);
        Assert.Contains("href: EVALUATION_PROTOCOL.md", toc, StringComparison.Ordinal);
        Assert.Contains("EVALUATION_PROTOCOL.md", commissionPackage, StringComparison.Ordinal);
        Assert.Contains("EVALUATION_PROTOCOL.md", acceptance, StringComparison.Ordinal);
        Assert.Contains("## Status issue #45", requirements, StringComparison.Ordinal);
        Assert.Contains("docs/EVALUATION_PROTOCOL.md", handoffScript, StringComparison.Ordinal);
        Assert.Contains("Kryteria akceptacji i oceny - Issue #45", pdfGenerator, StringComparison.Ordinal);
    }

    [Fact]
    public void Commission_pdf_is_versioned_and_has_a_valid_pdf_header()
    {
        var pdfPath = Path.Combine(
            RepositoryRoot,
            "output",
            "pdf",
            "Abituria-Technical-Documentation-0.9.0-beta.1.pdf");

        Assert.True(File.Exists(pdfPath), "Brak wygenerowanego PDF dla komisji.");
        var header = File.ReadAllBytes(pdfPath).Take(5).ToArray();
        Assert.Equal("%PDF-", System.Text.Encoding.ASCII.GetString(header));
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Abituria.sln"))) return directory.FullName;
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Nie znaleziono repozytorium Abituria.");
    }

    private static string Absolute(string relativePath) =>
        Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
}
