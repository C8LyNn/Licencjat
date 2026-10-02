# GitKraken — krótka instrukcja zespołu

## Codzienny start

1. Otwórz repozytorium.
2. Kliknij `Fetch All`.
3. Upewnij się, że `develop` jest aktualny.
4. Checkout własnej gałęzi `dev/<imie>`.
5. Wciągnij zmiany z `develop` do własnej gałęzi przed większą pracą.

## Commit

1. Wejdź w `WIP` / Unstaged Files.
2. Otwórz diff zmian.
3. Stage tylko pliki należące do jednego logicznego zadania.
4. Wpisz commit zgodny z `typ: opis`.
5. Commit.
6. Push.

## Pull Request

- Source: własna `dev/<imie>`.
- Target: `develop`.
- Opisz zmianę i sposób testowania.
- Dla grafiki/UI/levelu dodaj screenshot lub nagranie.
- Nie merge'uj samodzielnie do `main`.

## Konflikt

Jeśli konflikt dotyczy kodu, najpierw przeczytaj obie wersje i ustal poprawny rezultat.

Jeśli konflikt dotyczy `.unity`, `.prefab`, `.asset`, `.meta` albo dużej reorganizacji assetów — zatrzymaj operację i skonsultuj ją z osobą integrującą repo.

## Czego nie używać bez potrzeby

- Force Push
- Hard Reset
- Rebase na gałęzi współdzielonej
- Delete Remote Branch
- masowe Discard Changes
