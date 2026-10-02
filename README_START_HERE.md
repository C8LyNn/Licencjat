# START HERE — konfiguracja Git dla projektu Unity

1. Skopiuj **zawartość** tego startera do katalogu głównego istniejącego projektu Unity — tego, w którym są już `Assets/`, `Packages/` i `ProjectSettings/`.
2. Zainstaluj Git for Windows oraz Git LFS, jeśli nie są jeszcze zainstalowane.
3. Otwórz PowerShell w katalogu projektu.
4. W razie blokady lokalnych skryptów uruchom tylko dla tej sesji: `Set-ExecutionPolicy -Scope Process Bypass`.
5. Uruchom: `./Setup-GitUnity.ps1 -PersonalBranch "dev/jakub"`.
6. Utwórz puste repozytorium na GitHubie **bez** README, `.gitignore` i licencji.
7. Skopiuj jego adres HTTPS i uruchom ponownie: `./Setup-GitUnity.ps1 -PersonalBranch "dev/jakub" -RemoteUrl "https://github.com/USER/REPO.git"`.
8. Dodaj członków zespołu, utwórz ich `dev/<imie>` od `develop` i ustaw ochronę `main`/`develop`.

Nie uruchamiaj startera wewnątrz osobnego podfolderu projektu. Pliki `.gitignore`, `.gitattributes`, `Docs/` i skrypt mają znaleźć się w głównym katalogu projektu Unity.
