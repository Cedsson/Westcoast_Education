# WestCoast Education – Administration (Inlämningsuppgift 3)

Cross-platform-app för skrivbordet byggd med **Avalonia 11** och **MVVM** (CommunityToolkit.Mvvm).
Utbildningsledare och administratörer kan administrera kurser, studenter och lärare. All data sparas i **RestDb.io**.

## Funktioner
- **Kurser** – lista, lägga till och radera kurser som inte längre är aktuella
- **Studenter** – lista och lägga till
- **Lärare** – lista, lägga till och hantera en lärares kompetensområden (lägga till/ta bort)
- Ljust/mörkt tema. Alla färger och designvärden ligger i `Styles/Theme.axaml` och används som `DynamicResource`, så de byts direkt när temat ändras.

## Konfiguration av RestDb.io
1. Skapa tre collections i din RestDb.io-databas: `courses`, `students`, `teachers`. (Json för import finns i CollectionJson)
2. Fyll i databasens URL och en API-nyckel i appsettings.

