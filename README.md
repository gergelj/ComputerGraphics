# Projektni zadatak 6.2 – Ubacivanje CD-a u računar

> Computer Graphics

## Modelovanje statičke 3D scene (prva faza)

- [x] Uključiti testiranje dubine i sakrivanje nevidljivih površina. Definisati projekciju u perspektivi (fov=45, near=1, a vrednost far po potrebi) i viewport-om preko celog prozora unutar Resize metode.
- [x] Koristeći AssimpNet bibloteku i klasu *AssimpScene*, učitati model računara. Ukoliko je model podeljen u nekoliko fajlova, potrebno ih je sve učitati i iscrtati. Skalirati model, ukoliko je neophodno, tako da bude vidljiv u celosti.
- [x] Modelovati sledeće objekte:
  - [x] podlogu koristeći `GL_QUADS` primitivu,
  - [x] računarski sto korišćenjem *Cube* klase
  - [x] CD korišćenjem *Disk* klase
- [x] Ispisati 2D tekst žutom bojom u donjem desnom uglu prozora (redefinisati viewport korišćenjem `glViewport` metode). Font je Tahoma, 10pt, italic i underline. Tekst treba da bude oblika:

        Predmet: Racunarska grafika
        Sk.god: 2020/21.
        Ime: <ime_studenta>
        Prezime: <prezime_studenta>
        Sifra zad: <sifra_zadatka>

Predmetni projekat - faza 1 sačuvati pod nazivom: *PF1S6.2*. Obrisati poddirektorijume `bin` i `obj`. Zadaci se brane na vežbama, pred asistentima.

Vreme za izradu predmetnog projekta – faze 1 su dve nedelje. Predmetni projekat – faza 1 vredi 15 bodova. Način bodovanja je prikazan u tabeli.

| Šifra kriterijuma  | Bodovi         | Opis  |
|:-------------:|-------------:|:-----|
| CVP | 3 | Kreiran prozor. Uključeno testiranje dubine i sakrivanje nevidljivih površina. Projekcija, kliping volumen i viewport podešeni. |
|  M  | 9      | Adekvatno učitani ili modelovani pa zatim prikazani mesh modeli.|
|  T  | 3      | Ispisan tekst adekvatnim fontom, bojom, i na adekvatnoj poziciji. |

## Definisanje materijala, osvetljenja, tekstura, interakcije i kamere u 3D sceni (druga faza):

- [x] Uključiti color tracking mehanizam i podesiti da se pozivom metode `glColor` definiše ambijentalna i difuzna komponenta materijala.
- [x] Definisati tačkasti svetlosni izvor bele boje i pozicionirati ga iznad centra scene (na pozitivnom delu y-ose scene). Svetlosni izvor treba da bude stacionaran (tj. transformacije nad modelom ne utiču na njega). Definisati normale za podlogu. Za *Quadric* objekte podesiti automatsko generisanje normala.
- [x] Za teksture podesiti wrapping da bude `GL_REPEAT` po obema osama. Podesiti filtere za teksture tako da se koristi najbliži sused filtriranje. Način stapanja teksture sa materijalom postaviti da bude `GL_MODULATE`.
- [x] Stolu pridružiti teksturu drveta. Definisati koordinate tekstura.
- [x] Podlozi pridružiti teksturu tepiha (slika koja se koristi je jedan segment tepiha). Pritom obavezno skalirati teksturu (shodno potrebi).Skalirati teksturu korišćenjem Texture matrice.
- [x] Pozicionirati kameru, tako da se vide podloga, kao i bočna i zadnja strana aviona. Koristiti `gluLookAt` metodu.
- [x] Pomoću ugrađenih WPF kontrola, omogućiti sledeće:
    - [x] horizontalnu poziciju računara na stolu,
    - [x] izbor ambijentalne komponente reflektorskog izvora svetlosti, i
    - [x] izbor faktora (uniformnog) skaliranja računara.
- [x] Omogućiti interakciju sa korisnikom preko tastature: sa `F4` se izlazi iz aplikacije, tasterima `W`/`S` vrši se rotacija za 5 stepeni oko horizontalne ose, tasterima `A`/`D` vrši se rotacija za 5 stepeni oko vertikalne ose, a tasterima `+`/`-` približavanje i udaljavanje centru scene. Ograničiti rotaciju tako da se nikada ne vidi donja strana podloge. Dodatno ograničiti rotaciju oko horizontalne ose tako da scena nikada ne bude prikazana naopako.
- [x] Definisati reflektorski svetlosni izvor (cut-off=30º) crvene boje iznad računara, usmeren ka računaru.
- [x] Način stapanja teksture sa materijalom podloge postaviti na `GL_ADD`.
- [x] Kreirati animaciju ubacivanja CD-a u računar. Animacija treba da sadrži sledeće:
    - CD čitač računara se otvara.
    - CD se nalazi unutar njegovog ležište i polako počinje da usporava rotaciju.
    - Kada se potpuno zaustavi, ispada iz ležišta i čitač se zatvara.
    
    U toku animacije, onemogućiti interakciju sa korisnikom (pomoću kontrola korisničkog interfejsa i tastera). Animacija se može izvršiti proizvoljan broj puta i pokreće se pritiskom na taster `C`.

Neophodne teksture pronaći na internetu. Predmetni projekat - faza 2 sačuvati pod nazivom: *PF2S6.2*. Obrisati poddirektorijume `bin` i `obj`. Zadaci se brane na vežbama, pred asistentima.

Vreme za izradu predmetnog projekta - faze 2 su četiri nedelje. Predmetni projekat - faza 2 vredi 35 bodova. Način bodovanja je prikazan u tabeli.

| Šifra kriterijuma  | Bodovi         | Opis  |
|:-------------:|-------------:|:-----|
| M | 2 | Podešeni materijali u skladu sa zahtevima zadatka. |
|  S  | 8 | Definisani svetlosni izvori, u skladu sa zahtevima zadatka.|
|  T  | 8 | Učitane, dodeljene, podešene, i mapirane teksture, u skladu sa zahtevima zadatka. |
|  K  | 2 | Definisana kamera. |
|  I  | 7 | Omogućena interakcija, u skladu sa zadatkom. |
|  A  | 8 | Realizovana animacija, u skladu sa zadatkom. |

## Keyboard shortcuts

- `A`/`D` - rotate scene around the vertical axis (Y)
- `W`/`S` - rotate scene around the horizontal axis (X)
- `+`/`-` - Zoom In/Out
- `F4` - close the application
- `F2` - load another model
- `C` - begin animation