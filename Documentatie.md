Documentatie Tehnica: 
Aplicatie Web "Golden Era Movies"

TEMA: PLATFORMA DE RATING PENTRU FILME

1. Introducere
Golden Era Movies este o platforma web dedicata pasionatilor de cinematografie clasica. Aplicatia permite utilizatorilor sa exploreze filme, actori, sa gestioneze o lista personala de vizionare (Watchlist) si sa interactioneze prin recenzii si comentarii.

2. Functionalitatile Aplicatiei (Functional Requirements)
2.1. Modulul de Navigare (Navbar)
Logo Interactiv: Redirectioneaza utilizatorul catre pagina de profil.
Meniu Dinamic: Acces rapid catre sectiunile: Home, Movies, Top Movies, Actors, si Login/Register.
Bara de Cautare: Caseta de text pentru cautarea filmelor sau actorilor.

2.2. Pagina Principala (Home) 
Afisare Carduri Orizontale: Filmele sunt prezentate cu poster, titlu, an, rating si o scurta descriere.
Sistem de Rating: Afisarea scorului IMDB (ex: 9.2/10).
Lista este actualizata in functie de ratingul filmelor.

2.3. Detalii Film (Movie Template)
Sectiune Hero: Poster mare, sinopsis detaliat, regizor si distributie.
Actiuni Utilizator: Butoane pentru "+ Add to Watchlist" si "Comments".

2.4. Sistem de Recenzii (Comments Page)
Adaugare Comentariu: Caseta de text (textarea) cu redimensionare controlata (doar pe verticala) si validare (required).
Afisare Feedback: Lista de comentarii cu nume, data si textul recenziei.

2.5. Gestiune Actori (Actors - Hall of Fame)
Prezentare Vizuala: Lista verticala cu poze de profil rotunde (border-radius: 50%) si rama aurie.
Statistici: Afisarea rolului iconic si a numarului de premii Oscar.

2.6. Autentificare (Login & Register)
Hub de Acces: Pagina intermediara pentru alegerea intre Login si Creare Cont.
Formulare Validate: Campuri de tip email si password cu validare nativa de browser.

2.7. Profil Utilizator (Profile Page)
Informatii Personale: Avatar generat din initiale, nume si email.
Statistici Activitate: Contor pentru filme vizionate si recenzii postate.
Meniu Setari: Lista de actiuni (Schimbare parola, Watchlist, Logout) cu navigare tip "chevron" (&rsaquo;).

2.8.Lista Filme & Top Movies
Adaugat in baza de date este primul care apare in lista.
Filmele din sectiunea Top Movies sunt aranjate in functie de nr. de views.

3. Specificatii Tehnice (Technical Specifications)
3.1. Tehnologii Utilizate (Frontend)
HTML5: Structura semantica a paginilor.
CSS3: Stilul vizual, utilizand Flexbox pentru aliniere si CSS Grid pentru sectiunile de profil si liste. Am combinat Flexbox pentru alinierea elementelor din Navbar și carduri cu CSS Grid pentru layout-ul complex al paginii de profil.
Fonturi si Culori: Tema intunecata (Dark Mode) cu accente de "Gold" (#ffc107) si fundal gri inchis (#2c2c2c).

3.2. Arhitectura Fisierelor
Fisiere HTML separate pentru fiecare functionalitate (modularitate).
Fisiere CSS dedicate (ex: navbar.css, profile.css) pentru o intretinere usoara a codului.
Am ales o arhitectură modulară, cu fișiere HTML și CSS separate pentru fiecare funcționalitate. Acest lucru respectă principiul Separation of Concerns, făcând codul ușor de întreținut și de depanat.

4. Interfata Utilizator (UI/UX Features)
Responsive Design: Aplicatia foloseste max-width si flex-direction: column pentru a se adapta pe diferite rezolutii.
Feedback Vizual: Toate butoanele si link-urile au stari de hover clar definite.
Fiecare interacțiune, de la hover-ul pe carduri până la tranziția butoanelor, este gândită pentru a ghida utilizatorul.


