# Zasady Git i Unity — projekt licencjacki

## 1. Model gałęzi

- `main` — wyłącznie stabilna, działająca wersja projektu.
- `develop` — wspólna gałąź integracyjna zespołu.
- `dev/<imie>` — stała gałąź robocza konkretnej osoby.
- Zwykła praca trafia: `dev/<imie>` -> Pull Request -> `develop`.
- `develop` trafia do `main` tylko świadomie, po sprawdzeniu wersji integracyjnej.
- Nie pracujemy bezpośrednio na `main` ani `develop`.
- Finalne merge wykonuje opiekun repo / osoba odpowiedzialna za integrację.

## 2. Początek pracy

1. Otwórz repo w GitKraken.
2. `Fetch All`.
3. Zaktualizuj `develop`.
4. Przejdź na swoją gałąź `dev/<imie>`.
5. Wciągnij aktualny `develop` do swojej gałęzi przed rozpoczęciem większej pracy.
6. Dopiero wtedy edytuj projekt.

## 3. Koniec pracy

1. Zapisz i zamknij Unity, jeśli zmieniane były sceny/prefaby lub importowały się assety.
2. Sprawdź `git status` / listę zmian w GitKraken.
3. Obejrzyj diff każdego pliku tekstowego przed commitem.
4. Nie commituj plików wygenerowanych, cache ani zawartości `_Local`.
5. Zrób logiczny commit.
6. Push na własną gałąź.
7. Gdy zmiana jest gotowa — Pull Request do `develop`.

## 4. Standard commitów

Format:

`typ: konkretny opis zmiany`

Dozwolone typy:

- `feat:` — nowa funkcja / mechanika
- `fix:` — poprawka błędu
- `art:` — grafika, modele, animacje, audio lub inne assety
- `refactor:` — przebudowa bez zmiany zachowania
- `docs:` — dokumentacja
- `chore:` — konfiguracja, porządki, narzędzia

Przykłady:

- `feat: add player dash cooldown`
- `fix: prevent enemy spawn outside arena`
- `art: update forest environment props`
- `refactor: split inventory controller`
- `docs: describe level ownership rules`
- `chore: update Unity gitignore`

Nie używamy commitów typu `update`, `changes`, `stuff`, `test123`.

`WIP` może pojawić się na własnej gałęzi, ale przed PR historia ma być czytelna i zmiana ma mieć jasno opisany stan.

## 5. Pull Request

PR powinien zawierać:

- krótko: co zostało zmienione,
- po co została wykonana zmiana,
- jak została sprawdzona,
- znane problemy / rzeczy niedokończone,
- screenshot lub nagranie dla zmian wizualnych, UI, level designu lub animacji.

PR nie powinien łączyć wielu niezależnych zadań.

## 6. Code review

Komentarze review mogą używać oznaczeń:

- `[BLOCKER]` — trzeba poprawić przed merge,
- `[SUGGESTION]` — propozycja poprawy,
- `[QUESTION]` — pytanie / potrzeba wyjaśnienia,
- `[NIT]` — drobnostka, nie blokuje merge.

Nie zatwierdzamy PR bez przejrzenia realnych zmian.

## 7. Unity: sceny, prefaby i `.meta`

- Nie kasujemy ręcznie plików `.meta`.
- Przenoszenie i zmiana nazw assetów odbywa się w Unity, nie w Eksploratorze Windows.
- Asset i odpowiadający mu `.meta` zawsze idą razem.
- Dwie osoby nie powinny jednocześnie przebudowywać tej samej sceny lub dużego prefabu.
- Przed pracą nad dużą sceną/prefabem zespół ustala właściciela zadania.
- Konfliktów w scenach/prefabach nie rozwiązujemy metodą „wybierz dowolną wersję”. Jeśli konflikt jest niejasny, zatrzymujemy merge i wspólnie ustalamy poprawną wersję.

## 8. Git LFS

Git LFS służy do ciężkich i binarnych plików źródłowych, np. PSD/PSB, Blender/FBX, duże audio i wideo.

Nie dodajemy masowo nowych rozszerzeń do LFS bez uzgodnienia. Najpierw sprawdzamy, czy rzeczywiście są duże i binarne.

## 9. Pliki lokalne

`Assets/_Local/` i `ArtSource/_Local/` są ignorowane przez Git.

Można tam trzymać:

- tymczasowe testy,
- eksporty,
- prywatne notatki robocze,
- pliki pomocnicze,
- rzeczy, których nie chcemy wysyłać zespołowi.

Nic, od czego zależy działanie projektu, nie może istnieć wyłącznie w `_Local`.

## 10. Organizacja projektu

Główne miejsca:

- `Assets/_Project/` — wszystkie assety należące do właściwej gry,
- `ArtSource/` — źródłowe pliki graficzne i DCC, które nie muszą być bezpośrednio w `Assets`,
- `ThirdParty/` — zewnętrzne paczki i zasoby, jeśli sposób instalacji/licencja na to pozwala,
- `Docs/` — dokumentacja techniczna i zespołowa,
- `Assets/_Local/` — prywatne pliki robocze, ignorowane przez Git.

W `Assets/_Project/` tworzymy katalogi według funkcji projektu, nie według osób. Nie tworzymy folderów `Kuba`, `Grafik1`, `Moje`, `Nowe2` itp.

Przykładowo:

- `Art/`
- `Audio/`
- `Code/`
- `Prefabs/`
- `Scenes/`
- `UI/`
- `VFX/`

Nie duplikujemy assetów tylko po to, by „mieć własną wersję”.

## 11. Kod i komentarze

- Nazwy mają mówić, czym coś jest lub co robi.
- Komentarz ma przede wszystkim wyjaśniać **dlaczego**, jeśli kod sam nie wyjaśnia intencji.
- Nie komentujemy oczywistości.
- TODO powinno być konkretne, np. `TODO: replace temporary spawn rule after arena data is available`.
- Tymczasowe obejścia powinny być oznaczone i opisane.
- Nie zostawiamy martwego kodu „na wszelki wypadek” — historię zapewnia Git.

## 12. Operacje bezpieczne w GitKraken

Każdy członek zespołu może samodzielnie wykonywać:

- Fetch,
- Pull,
- Push na własną gałąź,
- Stage / Unstage,
- Commit,
- przeglądanie diffów,
- tworzenie Pull Requestów,
- zwykłe przełączanie gałęzi.

Początkująca osoba nie wykonuje samodzielnie:

- Force Push,
- Hard Reset,
- Rebase współdzielonej historii,
- kasowania zdalnych gałęzi,
- przepisywania opublikowanej historii,
- rozwiązywania dużych konfliktów scen/prefabów bez konsultacji,
- merge do `main`.

## 13. Zasada awaryjna

Jeśli Git pokazuje coś niezrozumiałego, **nie klikamy losowo**. Najpierw robimy screenshot, zatrzymujemy operację i ustalamy, co dokładnie się stało.
