<p align="center">
  <img src="tisztavaros_logo.png" alt="Tiszta Város logó" width="200"/>
</p>

# Tiszta Város – Admin Felület (WPF)

## Projektleírás
A **Tiszta Város** egy közösségi városüzemeltetési platform, melynek célja a települések tisztaságának javítása a lakosság bevonásával.  
Ez a WPF alapú **Windows asztali adminisztrációs alkalmazás** a városi adminisztrátorok és ügyintézők számára készült, hogy a beérkezett bejelentéseket és a rendszer adatait hatékonyan kezelhessék.  

Az admin felületen keresztül nyomon követhetők a bejelentések, módosíthatók az adatok, valamint menedzselhetők a felhasználók, kategóriák, intézmények és gamifikációs kihívások.  
A háttérben egy külön [backend](https://github.com/Fruktoz0/backend) szolgáltatás működik, amely Node.js alapú REST API-t és MySQL adatbázist használ.

## Fő funkciók
- **Felhasználókezelés** – regisztrált felhasználók listázása, jogosultságok és státusz módosítása.  
- **Bejelentések kezelése** – beérkezett bejelentések áttekintése, státuszváltoztatás, továbbítás intézményekhez.  
- **Kategóriák kezelése** – bejelentéstípusok létrehozása, módosítása, törlése.  
- **Intézmények kezelése** – intézményi adatok és hozzárendelt felhasználók kezelése, logók feltöltése.  
- **Kihívások kezelése** – gamifikációs feladatok létrehozása és felhasználói teljesítések nyomon követése.  

## Technológiai stack
- **C# és .NET 8 (WPF)**
- **REST API kommunikáció** (HttpClient, JWT authentikáció)
- **MySQL** (közvetlenül a backend kezeli)
- **Newtonsoft.Json** (JSON feldolgozás)
- **GMap.NET + OpenStreetMap** (térképes megjelenítés)

## Fejlesztés és futtatás

### Előfeltételek
- Windows 10/11  
- .NET 8 SDK  
- Visual Studio 2022  
- Node.js + npm (a backend futtatásához)  
- MySQL adatbázis  

### Telepítés és indítás
1. **Backend beállítása és indítása**
   ```bash
   git clone https://github.com/Fruktoz0/backend.git
   cd backend
   npm install
   npm start
   ```
   Győződj meg róla, hogy a backend fut és kapcsolódik a MySQL adatbázishoz.

2. **Admin kliens klónozása**
   ```bash
   git clone https://github.com/Fruktoz0/tisztavaros-wpf.git
   cd tisztavaros-wpf/TisztaVaros
   ```

3. **Build és futtatás**
   Visual Studio-ban nyisd meg a `TisztaVaros.sln`-t és fordítsd le, vagy parancssorból:
   ```bash
   dotnet build TisztaVaros.sln
   dotnet run --project TisztaVaros/TisztaVaros.csproj
   ```

4. **Bejelentkezés**
   - Alapértelmezett admin adatok:  
     - Email: `admin@admin.hu`  
     - Jelszó: `admin`  
   - A *“HTTP Local”* opcióval választhatsz a helyi (`localhost`) vagy a távoli szerverhez való kapcsolódás között.

## Kapcsolódó repók
- **Backend:** [Fruktoz0/backend](https://github.com/Fruktoz0/backend)
