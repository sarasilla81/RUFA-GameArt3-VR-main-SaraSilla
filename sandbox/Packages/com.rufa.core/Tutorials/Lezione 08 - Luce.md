Lezione 8 · La luce cotta nel faro. Le note prima del primo capitolo non si vedono nella finestra. Stima per lo studente lento (tools/stima-tempi.py): capitoli insieme 0-4 circa 109', il laboratorio (capitolo 5) circa 43', verifica circa 43'. La giornata è negli appunti del docente. Parla Gianluca in prima persona e dà del tu allo studente. Nessun numero misurato su un PC: solo quelli di Meta e di Unity, e quelli costruiti nella scena (i budget dei cartelli). Ogni stanza è una scena, perché Unity cuoce tutta la scena aperta. Si misura in Play, sul Display 2 della stanza; si cura fuori dal Play.

# 0 · Prima di iniziare

## Cosa farai in questa lezione
Oggi il cliente è una società che fa escape room in realtà virtuale. Ha restaurato il vecchio faro dell'isola e lo vuole sul Quest 3S, di sera, con le lampade accese.
Il problema è la luce. Ogni lampada costa al visore a ogni frame, e se fa ombra costa molto di più. Sul visore, anzi, le ombre delle lampade non ci sono nemmeno: nel nostro progetto sono spente.
Oggi impari a **cuocere** la luce: Unity la calcola una volta sola, prima, e la dipinge sugli oggetti fermi. Le ombre tornano morbide, la luce rimbalza sui muri, e mentre il gioco gira non costa quasi niente.
Il soggiorno lo cuociamo insieme. Dopo la pausa rifai in Blender le UV della luce di uno scaffale. Poi cuoci da solo l'officina, la scala e la lanterna. Ti capiterà davvero: Meta racconta che Batman: Arkham Shadow ha la luce cotta in tutto il gioco.
> lavoro: 1 1

## I modelli nuovi
Questa pagina controlla che nel tuo progetto ci siano i modelli del faro, in `Assets/Demo/Models/Faro`.
Se qui sotto c'è scritto **Fatto**, vai avanti. Se resta **Da fare**, chiudi Unity, lancia `git aggiorna` un'altra volta nella cartella della tua copia e riapri il progetto. Il primo `git aggiorna` di oggi porta il comando nuovo, il secondo porta i file. Unity Hub deve essere aperto: il progetto deve riscaricare i suoi package.
> verifica: asset-esiste Assets/Demo/Models/Faro/scaffale.fbx

## Il rig del PC
Oggi misuri in Play. Il visore resta spento: usiamo il rig del PC.
1. Dal menu in alto apri **RUFA ▸ Play mode**. Se accanto a **Simulatore VR** c'è la spunta, clicca la voce: la spunta sparisce.
In Play clicca nella Game view per guardare col mouse e cammina con **W A S D**. **Esc** ti ridà il mouse.
> verifica: simulatore-spento

## L'editor sul PC
I budget di oggi sono pensati per l'editor impostato per il PC. Se l'ultima volta hai fatto una build per il visore, l'editor è rimasto su Android e i numeri vengono diversi.
Se qui sotto resta **Da fare**:
1. Dal menu in alto apri **File ▸ Build Profiles**.
2. A sinistra, fra le piattaforme, scegli **Windows** e premi **Switch Platform**. Unity ci mette qualche minuto.
> verifica: piattaforma-pc

## Apri il faro
Il faro ha quattro stanze, e ogni stanza è una scena: il soggiorno, l'officina, la scala e la lanterna. Le scene sono separate per un motivo che vedrai fra poco: quando cuoci la luce, Unity cuoce tutta la scena aperta.
Premi il pulsante qui sotto. Unity copia il faro nel tuo progetto, in `Assets/Samples/RUFA Core/Lezione 08 - Luce`, e apre il soggiorno.
La copia è tua: puoi cambiarla quanto vuoi. Se la rovini, cancella quella cartella e premi di nuovo il pulsante.
> azione: Apri il faro | apri-scena | Lezione 08 - Luce | Soggiorno
> verifica: scena-aperta Soggiorno

## La camera della stanza
Ogni stanza ha una camera fissa, che guarda dentro.
1. Clicca la linguetta **Game** per mettere la Game view davanti.
2. In alto a sinistra nella Game view c'è un menu con scritto **Display 1**. Aprilo e scegli **Display 2**: compare il soggiorno, visto dalla sua camera.
3. Accanto c'è il menu delle proporzioni. Scegli **16:9 Aspect**, così l'inquadratura è la stessa su tutti i PC.
4. In alto a destra nella Game view premi **Stats**. Compare un riquadro coi numeri del frame.
Nel soggiorno c'è anche il **Display 3**: lo scaffale, da vicino. Ti serve nel laboratorio. Il **Display 1** è il tuo rig, in Play.
> lavoro: 2 1

## Il tuo quadro
Sopra il divanetto, sulla parete di fondo, c'è un quadro con la tela bianca. È per te. Se hai un tuo disegno, una tua grafica o una tua foto, mettila lì.
1. Trascina il file della tua immagine, PNG o JPG, dall'Esplora file alla cartella `Assets/Progetto` nella finestra **Project**.
2. Nella Hierarchy apri `Stanza` e seleziona `Tela`.
3. In fondo all'Inspector c'è il materiale `Il tuo quadro`. Aprilo con la freccia. Accanto a **Base Map** c'è un quadratino: trascinaci sopra la tua immagine.
Stasera la tua immagine prende la luce calda delle lampade. Se oggi non hai un'immagine con te, salta questa pagina.
> lavoro: 3 2

## Il file delle misure
Oggi scrivi i numeri che leggi in un file, `Consegne/L08/misure.md`. Premi il pulsante: il file si crea e si apre nel blocco note. Tienilo aperto, ci torni spesso.
> azione: Crea il file delle misure | crea-misure | Consegne/L08/misure.md
> verifica: file-esiste Consegne/L08/misure.md

# 1 · Quanto costa la luce

## La luce in tempo reale
Una luce **in tempo reale** si calcola a ogni frame, su ogni pixel che tocca. Per questo URP, sui dispositivi mobili, accende al massimo 9 luci per oggetto, e nel nostro progetto 5: quella principale e 4 aggiuntive.
Le **ombre** costano di più. Per sapere dove cade l'ombra, la luce disegna prima la scena dal suo punto di vista, in una **mappa delle ombre**: ogni oggetto che fa ombra si disegna una volta in più. Una lampada che illumina in tutte le direzioni, una **luce puntiforme**, di mappe ne fa sei. Unity scrive che costa come sei luci spot, e che sui dispositivi mobili va ridotta o evitata.

## Le regole di Meta
Meta dà alle app per il Quest regole semplici:
- cuoci la luce ogni volta che puoi;
- le ombre dinamiche, quelle calcolate mentre il gioco gira, possono avere un impatto enorme sulle prestazioni: usa la luce cotta;
- per i personaggi e le cose che si muovono, usa le light probe al posto delle luci dinamiche;
- spegni le ombre quando ti avvicini ai limiti di geometria o di draw call.
Il nostro cliente l'ha tradotto nel cartello che vedi in ogni stanza: un budget di **Batches** e il numero di luci che possono restare in tempo reale. Qui nel soggiorno, zero.

## Le luci della stanza
Prima di curare, conta. Unity ha una finestra che elenca tutte le luci della scena.
1. Dal menu in alto apri **Window ▸ Rendering ▸ Light Explorer**.
2. Nella scheda **Lights** c'è una riga per ogni luce: le quattro lampade e il sole. Allarga la finestra: la colonna **Mode** dice **Realtime** per tutte.
3. Scrivi nel file delle misure quante luci sono in tempo reale.
Tieni aperto il Light Explorer: lo usi ancora.
> lavoro: 2 1

## Si misura in Play
1. Premi **Play**.
2. Nella Game view scegli **Display 2**: il soggiorno. Se il menu non risponde, premi **Esc** per riavere il mouse.
3. Nel riquadro **Stats** leggi **Batches** e **Shadow casters**, gli oggetti che fanno ombra. Scrivili nel file delle misure, alla riga del soggiorno.
4. Confronta **Batches** col budget sul cartello. È molto più alto.
5. Premi di nuovo **Play** per uscire.
Come nella lezione 7: misuri in Play, curi fuori dal Play. Se nella **Console** compare un avviso sullo **shadow atlas**, URP ti dice che le mappe delle ombre sono così tante che ha dovuto rimpicciolirle.
> verifica: fuori-play
> lavoro: 3 2

## Sul visore
Ora guarda come il progetto disegna sul Quest.
1. Nella finestra **Project** apri `Assets/Settings` e seleziona `Mobile_RPAsset`: sono le impostazioni di URP per il visore.
2. Nell'Inspector apri **Lighting**. Sotto **Additional Lights**, cioè le luci oltre al sole, la casella **Cast Shadows** è spenta.
Vuol dire che sul visore le lampade del faro non fanno ombra: la stanza sembra piatta, e le cose non poggiano sul tavolo. Non cambiare niente: è la scelta giusta per il Quest. Il bake ti ridà quelle ombre senza costi.
> lavoro: 2 1

## La foto del prima
La foto è un'immagine di quello che vede la camera del soggiorno. Il pulsante la salva in `Consegne/L08/soggiorno-prima.jpg`, accanto al file delle misure.
> azione: Foto del soggiorno, prima | foto-camera | Camera Soggiorno | Consegne/L08/soggiorno-prima.jpg
> verifica: file-esiste Consegne/L08/soggiorno-prima.jpg

# 2 · Cuocere la luce

## I tre modi di una luce
Ogni luce ha un **Mode**:
- **Realtime**: calcolata a ogni frame. Costa più di tutte, e le sue ombre sono nere, senza luce che rimbalza.
- **Mixed**: un po' cotta e un po' in tempo reale. Unity scrive che costa come il tempo reale.
- **Baked**: cotta. Unity calcola una volta sola la luce diretta e quella che rimbalza, e la scrive nelle **lightmap**, immagini dipinte sopra gli oggetti fermi. Mentre il gioco gira non costa niente.
Una luce cotta non fa i riflessi lucidi, e non illumina le cose che si muovono: per quelle ci sono le light probe, nel capitolo 3. Una luce che si muove, o che un interruttore accende, resta in tempo reale. Nel soggiorno cuociamo tutto, anche il sole: il tramonto non si muove.

## Cosa è fermo
La luce cotta si dipinge solo sugli oggetti che Unity sa fermi: gli **statici**. Muri, pavimento, mobili e lampade non si muovono: diventano statici. Le cose sul tavolo si prendono in mano: restano libere. E restano liberi i vetri, che nel bake fermerebbero il sole.
1. Nella Hierarchy seleziona `Stanza`.
2. In alto a destra nell'Inspector c'è la casella **Static**. Mettici la spunta.
3. Unity chiede se cambiare anche i figli: rispondi **Yes, change children**.
Fra le voci di **Static** ci sono **Contribute GI**, che mette l'oggetto nel bake, e **Reflection Probe Static**, che lo fa vedere nei riflessi cotti. Con la spunta su **Static** le hai accese tutte.
Se la pagina resta su **Da fare**: un oggetto della stanza non è statico, oppure una cosa del tavolo lo è, oppure qualcosa è stato spostato. Annulla con **Ctrl+Z**.
> verifica: statici-luce Stanza | Oggetti
> lavoro: 2 1

## Anche lo scaffale
Lo scaffale sta in una radice sua, `Laboratorio`, perché dopo la pausa lo riesporti da Blender. Rendilo statico allo stesso modo: seleziona `Laboratorio`, spunta **Static**, **Yes, change children**.
> verifica: statici-luce Laboratorio | Oggetti
> lavoro: 1 1

## Le UV della luce
Per dipingere la lightmap su un oggetto, Unity deve sapere dove va ogni pezzo della sua superficie. Ogni oggetto statico ha quindi una seconda mappa UV, solo per la luce: le **UV della luce**. I cubi dei muri le hanno già; per i modelli le genera Unity.
Nella finestra **Project** apri `Assets/Demo/Models/Faro` e seleziona `dining_table`, il tavolo. Nell'Inspector, scheda **Model**, c'è la spunta a **Generate Lightmap UVs**. È accesa per tutti i modelli del faro tranne lo scaffale: il motivo lo scopri dopo il bake.
> lavoro: 1 1

## Le luci in Baked
1. Nella Hierarchy apri `Luci`. Clicca la prima luce, poi **Shift**+clic sull'ultima: sono tutte selezionate.
2. Nell'Inspector, nel componente **Light**, metti **Mode** su **Baked**.
Nel Light Explorer la colonna **Mode** ora dice **Baked** per tutte.
Se la pagina resta su **Da fare**: una luce è rimasta Realtime o Mixed, oppure è stata spenta o spostata.
> verifica: luci-cotte Luci
> lavoro: 2 1

## La brace
Nella stufa c'è la brace, e sulla radio il quadrante acceso. Sono superfici che brillano: hanno l'**emissione**. Possono illuminare la stanza anche loro, ma solo se entrano nel bake.
1. Nella Hierarchy, dentro `Stanza`, seleziona `Brace`.
2. In fondo all'Inspector apri il materiale `Brace` con la freccia. Sotto **Emission** c'è **Global Illumination**: dice **Realtime**. Vuol dire che questa luce conta solo per la GI in tempo reale, che nel progetto è spenta. Mettila su **Baked**.
> verifica: emissione-cotta Brace
> lavoro: 2 1

## La radio
Fai lo stesso col quadrante della radio: seleziona `Quadrante`, apri il suo materiale e metti **Global Illumination** su **Baked**.
> verifica: emissione-cotta Quadrante
> lavoro: 1 1

## La finestra Lighting
Le regole del bake stanno nella finestra **Lighting**. Un valore alla volta.
1. Dal menu in alto apri **Window ▸ Rendering ▸ Lighting**, scheda **Scene**. In **Lighting Settings Asset** c'è `Soggiorno`: ogni stanza ha il suo.
2. Sotto **Mixed Lighting** la spunta a **Baked Global Illumination** è accesa: è la luce cotta.
3. Sotto **Lightmapping Settings**, **Lightmapper** dice **Progressive GPU**: cuoce con la scheda grafica. Se il tuo PC non ce la fa, Unity passa da solo al processore, più lento.
4. **Lightmap Resolution** è quanti pixel di lightmap ci sono per ogni metro di superficie. Unity parte da 40: per una stanza vista nel visore è tanto. Mettila a **10**. Raddoppiarla vuol dire quattro volte i pixel.
5. **Max Lightmap Size** è la grandezza massima di ogni lightmap: lascia **1024**.
6. **Lightmap Compression**: scegli **Normal Quality**, un buon compromesso fra memoria e qualità.
7. **Directional Mode**: scegli **Non-Directional**. Meta lo chiede per il Quest: le lightmap direzionali occupano circa il doppio.
> lavoro: 3 2

## Cuoci
In fondo alla finestra **Lighting** premi **Generate Lighting**. In basso a destra nell'editor compare una barra: Unity sta cuocendo. La prima volta la scheda grafica si deve preparare, e ci mette di più.
Quando la barra sparisce, guarda la riga sotto il pulsante: dice quante lightmap ha fatto, e quanta memoria occupano.
Se la pagina resta su **Da fare** anche dopo il bake: un oggetto è diventato statico dopo, e non ha ancora la sua lightmap. Premi di nuovo **Generate Lighting**.
> attesa: 60
> verifica: luce-cotta
> lavoro: 2 1

## Guarda la luce cotta
Guarda il soggiorno nella Game view, sul **Display 2**. Le ombre sono morbide. Il sole entra dalla finestra e la sua luce rimbalza sul pavimento e sui muri. Gli angoli e il sotto del tavolo sono più scuri, come in una stanza vera.
La stanza è anche più scura e più calda di prima. Prima la luce del cielo arrivava ovunque, anche dentro una stanza chiusa; cotta, entra solo dalla finestra. E la luce che rimbalza sul legno prende il colore del legno.
Ora guarda le lightmap da sole.
1. Nella Scene view, in alto, c'è la barra **Draw Modes**: quattro icone a forma di sfera, poi un insetto con una freccina. Se passi col mouse sopra l'insetto, compare la scritta **Debug Draw Mode**. Cliccalo.
2. Nel gruppo **Baked Global Illumination** scegli **Baked Lightmap**: vedi solo la luce cotta, senza i colori dei materiali.
3. Per tornare come prima, clicca la quarta sfera: passandoci sopra dice **Shaded Draw Mode**.
Guarda anche lo scaffale, sul **Display 3**: sul ripiano più basso c'è un'ombra scura, ma sopra non c'è niente che la faccia. È il problema che curi dopo la pausa.
Se la stanza ti sembra troppo buia, seleziona una lampada in `Luci`, alza **Intensity** nel componente **Light** e cuoci di nuovo. L'atmosfera è quella del cliente: la luce calda delle lampade, il buio negli angoli, il tramonto.
> lavoro: 3 2

# 3 · Le cose che si muovono

## Al buio
1. Premi **Play** e guarda il tavolo sul **Display 2**. La torcia, il binocolo, la bussola, il bollitore: sono più scuri di prima, piatti, come staccati dalla stanza.
2. Esci dal Play.
Sono le cose che si prendono in mano: non sono statiche, quindi non hanno la lightmap. E le lampade cotte non le illuminano più. Prendono solo la luce del cielo.

## Le light probe
Le **light probe** sono punti nello spazio vuoto della stanza. Durante il bake ognuno registra la luce che passa di lì, da ogni direzione. Una cosa che si muove prende la luce dalle quattro probe attorno a lei. Meta lo chiede: per i personaggi e le cose che si muovono, light probe al posto delle luci dinamiche.
Le regole di Unity:
- poche, ma abbastanza da registrare dove la luce cambia: più fitte al bordo di un'ombra o vicino a una lampada, rade dove la luce resta uguale;
- almeno due strati in altezza: un volume, non un tappeto;
- vicino a un muro, due probe per parte, perché la luce non lo attraversi.

## Il gruppo di probe
1. Dal menu in alto apri **GameObject ▸ Light ▸ Light Probe Group**. Nella Scene view compaiono otto palline, gli angoli di un cubo: sono le probe.
2. Nella Scene view, a sinistra, c'è la colonna degli strumenti. Sotto quelli per muovere e ruotare è comparsa l'icona gialla del gruppo, e sotto ancora un pulsante: **Edit Light Probe Group**. Premilo. In basso a destra nella Scene view compare il riquadro **Edit Light Probes**: ora puoi selezionare le probe una per una.
3. Sistemale attorno al tavolo, dove stanno le cose che si muovono: quattro all'altezza del piano, quattro più in alto, sopra il tavolo. Selezionane una e spostala con le frecce, come un oggetto.
4. Per aggiungerne altre, selezionale e nel riquadro premi **Duplicate Selected**: si duplicano, e le sposti. Mettine qualcuna anche vicino alla stufa e alla finestra, dove la luce cambia.
5. Tienile fuori dai mobili e dai muri.
> lavoro: 6 4

## Ricuoci
Le probe si cuociono col resto. Nella finestra **Lighting** premi di nuovo **Generate Lighting**.
Se la pagina resta su **Da fare**: le probe sono meno di otto, oppure una delle cose sul tavolo non ne ha almeno quattro vicine. Aggiungine attorno al tavolo e ricuoci.
> attesa: 60
> verifica: probe-attorno Oggetti | 8
> lavoro: 1 1

## Porta il binocolo
1. Premi **Play**. Stavolta guarda sul **Display 1**, col tuo rig: clicca nella Game view e cammina fino al tavolo.
2. Guarda il binocolo col mirino e tieni premuto il tasto sinistro del mouse: lo prendi. Sempre col tasto premuto, portalo vicino alla stufa, poi davanti alla finestra. Cambia colore: prende la luce delle probe vicine. Quando lasci il tasto, cade.
3. Esci dal Play.
Ogni cosa che si muove nel tuo gioco, compresi le mani e i personaggi, prende la luce così.
> lavoro: 2 1

# 4 · I riflessi

## Il cielo nello specchio
Guarda lo specchio sulla parete e il bollitore sul tavolo: riflettono il cielo, dentro una stanza chiusa. Unity scrive che un oggetto fuori da ogni **reflection probe** riflette il cielo. E le luci cotte non fanno i riflessi lucidi: senza una probe, gli ottoni perdono la luce delle lampade.
La **reflection probe** fotografa la stanza nelle sei direzioni, e le cose lucide riflettono quella foto. Cotta, è compressa e costa poco; in tempo reale si rifà di continuo, e **Every Frame** è l'opzione più cara.
Attenzione: la reflection probe la leggono i materiali **Lit**, non i **Simple Lit**. Per questo lo specchio e gli ottoni del faro sono Lit, e il resto è Simple Lit.

## La reflection probe
1. Dal menu in alto apri **GameObject ▸ Light ▸ Reflection Probe**.
2. Nell'Inspector, **Type** è **Baked**: lascialo.
3. Metti la probe al centro della stanza, all'altezza degli occhi: nel **Transform**, **Position** a `0`, `1.5`, `0`.
4. **Box Size** è la scatola che la probe riguarda: falla grande come la stanza, `6.4`, `3.1`, `5.4`.
5. Metti la spunta a **Box Projection**. Senza, la probe fa finta che la stanza sia infinitamente grande, e il riflesso nello specchio non torna. In URP va accesa anche nell'asset, e nel nostro progetto lo è.
6. **Blend Distance** a `0.1`. È la fascia, vicino alle pareti della scatola, dove il riflesso sfuma nel cielo. Col valore di Unity, `1`, lo specchio appeso al muro starebbe nella fascia, e rifletterebbe ancora il cielo.
La probe vede solo gli oggetti con **Reflection Probe Static**: con **Static** li hai già.
> lavoro: 3 2

## Cuoci i riflessi
Premi **Generate Lighting**. Unity cuoce anche la reflection probe. Guarda lo specchio: ora riflette la stanza.
Se la pagina resta su **Da fare**: la probe non è **Baked**, non ha **Box Projection**, è più piccola della stanza, ha ancora **Blend Distance** a `1`, oppure non è ancora cotta.
> attesa: 60
> verifica: riflesso-cotto Stanza
> lavoro: 1 1

## Il dopo del soggiorno
1. Premi **Play**, scegli **Display 2** e rileggi **Batches** e **Shadow casters**. Scrivili nel file delle misure ed esci dal Play.
2. Nel Light Explorer conta le luci in tempo reale: zero. Scrivi anche quanto è durato il bake.
3. Fai la foto del dopo.
4. Salva la scena con **Ctrl+S**.
> azione: Foto del soggiorno, dopo | foto-camera | Camera Soggiorno | Consegne/L08/soggiorno-dopo.jpg
> verifica: file-esiste Consegne/L08/soggiorno-dopo.jpg

## Il curatore
Il curatore è il tuo compagno di banco, e tu lo fai per lui.
1. Apri la cartella `Consegne/L08` nell'Esplora file. Ci sono le due foto del soggiorno.
2. Mostragliele una alla volta a tutto schermo.
3. Stavolta il compagno sceglie la foto che metterebbe nel gioco, e dice perché.
La luce cotta di solito vince. Ma deve restare l'atmosfera del cliente: se la stanza è troppo buia, alza l'intensità delle lampade e ricuoci; se ha perso il buio negli angoli, abbassala. Scrivi nel file cosa ha scelto. Poi scambiatevi i ruoli.
> lavoro: 3 2

# 5 · Il laboratorio

## Lo scaffale macchiato
Guarda lo scaffale da vicino: nella Game view scegli **Display 3**. Sul ripiano più basso c'è un'ombra scura, larga come la fila di libri del ripiano in alto. Sopra non c'è niente che la faccia: è proprio l'ombra di quei libri.
Il motivo sta nelle UV della luce dello scaffale. Chi l'ha modellato ha copiato le UV della texture: lì i quattro ripiani, che sono uguali, usano lo stesso pezzo di immagine. È un trucco giusto per la texture, perché risparmia spazio. Per la luce è sbagliato: Unity dipinge i quattro ripiani nello stesso pezzo di lightmap, e ognuno prende le ombre degli altri.
Fai la foto del prima.
> azione: Foto dello scaffale, prima | foto-camera | Camera Scaffale | Consegne/L08/scaffale-prima.jpg
> verifica: file-esiste Consegne/L08/scaffale-prima.jpg

## Le regole delle UV della luce
Unity scrive che le UV della luce fatte a mano devono:
- stare dentro il quadrato da 0 a 1;
- non avere facce sovrapposte;
- lasciare un margine fra un'isola e l'altra, perché la luce di un pezzo non coli sul vicino;
- deformare poco gli angoli e le aree, rispetto al modello.
Unity le sa generare da solo, con **Generate Lightmap UVs**, e per molti oggetti basta. In molti studi, però, le UV della luce le consegna l'artista, e le regole sono queste. Lo scaffale ne sbaglia una: i ripiani si sovrappongono.
> lavoro: 2 1

## In Blender · importa
1. Apri Blender. Col mouse sulla vista 3D premi **A**, poi **X**, e scegli **Delete**: via il cubo, la camera e la luce.
2. Dal menu **File ▸ Import ▸ FBX (.fbx)** apri `scaffale.fbx`. Sta nella cartella della tua copia, in `sandbox/Assets/Demo/Models/Faro`. Premi **Import FBX**.
3. Nell'Outliner, in alto a destra, clicca `Scaffale`.
> lavoro: 3 2

## In Blender · la mappa della luce
1. A destra, nelle **Properties**, apri la scheda **Object Data**, quella col triangolo verde. Il riquadro **UV Maps** è chiuso: clicca il suo titolo. Ci sono `UVMap` e `Lightmap`.
2. Clicca una volta il nome `Lightmap`: la riga si evidenzia. Non cliccare l'icona della fotocamera: deve restare su `UVMap`, la mappa della texture.
3. In alto passa alla scheda **UV Editing**: Blender entra da solo in Edit Mode. A sinistra c'è l'editor delle UV, a destra la vista 3D.
4. Col mouse sulla vista 3D premi **A**: tutto selezionato. L'editor delle UV mostra la mappa `Lightmap`.
> lavoro: 3 2

## In Blender · trova le sovrapposizioni
Nell'editor delle UV, a sinistra, apri **Select ▸ Select Overlap**. Si selezionano solo i ripiani: le loro facce stanno una sopra l'altra.
> lavoro: 1 1

## In Blender · Lightmap Pack
1. Col mouse sulla vista 3D premi **A**, perché sia di nuovo tutto selezionato.
2. Nel menu in alto della vista 3D apri **UV ▸ Lightmap Pack**. Si apre una finestrella:
   - **Selection**: **Selected Faces**;
   - **Share Texture Space**: acceso;
   - **New UV Map**: spento. Acceso, aggiungerebbe una terza mappa invece di rifare `Lightmap`;
   - **Pack Quality**: lascia **12**;
   - **Margin**: scrivi **0.5**. È lo spazio fra le facce.
3. Premi **OK**. Ogni faccia ha ora il suo posto nel quadrato.
4. Nell'editor delle UV apri di nuovo **Select ▸ Select Overlap**: non si seleziona niente.
Questo comando non ha il riquadro per cambiare i valori dopo: se vuoi un altro margine, **Ctrl+Z** e rifallo.
> lavoro: 4 3

## In Blender · esporta
1. Col mouse sulla vista 3D premi **Tab** per tornare in Object Mode.
2. **File ▸ Export ▸ FBX (.fbx)**.
3. A destra, sotto **Include**, spunta **Selected Objects**. Sotto **Transform**, in **Apply Scalings**, scegli **FBX Units Scale**.
4. Salva sopra `scaffale.fbx`, nella cartella `sandbox/Assets/Demo/Models/Faro`, con lo stesso nome. Premi **Export FBX**.
Se qualcosa è andato storto, ricomincia in un file nuovo: **File ▸ New ▸ General**, e importa di nuovo il modello. Se l'hai già sovrascritto male, nella cartella della tua copia lancia `git checkout -- sandbox/Assets/Demo/Models/Faro/scaffale.fbx` e il modello torna com'era.
> lavoro: 3 2

## In Unity · le UV nuove
Torna in Unity: il modello si aggiorna da solo. Questa pagina controlla le sue UV della luce: che siano le tue, senza sovrapposizioni, e che Unity non le rifaccia da solo.
Se resta **Da fare**: in Blender la mappa rifatta non era `Lightmap`, oppure **New UV Map** era acceso, oppure qualche faccia si sovrappone ancora. Rifai il **Lightmap Pack** con un **Margin** più grande.
> verifica: uv-luce Assets/Demo/Models/Faro/scaffale.fbx
> lavoro: 2 1

## In Unity · ricuoci
Nella finestra **Lighting** premi **Generate Lighting**.
> attesa: 60
> verifica: luce-cotta
> lavoro: 1 1

## Lo scaffale · il dopo
Guarda lo scaffale sul **Display 3**: l'ombra sul ripiano più basso è sparita, e ogni ripiano ha la sua luce.
Fai la foto del dopo e mostrala al curatore insieme a quella del prima. Salva la scena con **Ctrl+S**.
> azione: Foto dello scaffale, dopo | foto-camera | Camera Scaffale | Consegne/L08/scaffale-dopo.jpg
> verifica: file-esiste Consegne/L08/scaffale-dopo.jpg

## Trucchi del mestiere · nel programma 3D
- **Una seconda UV per ogni oggetto fermo.** È la prima cosa che uno studio controlla in un modello che riceve la luce cotta.
- **Niente isole impilate o specchiate nella mappa della luce.** Nella mappa della texture vanno bene, e fanno risparmiare spazio. In quella della luce ogni faccia vuole il suo posto.
- **Il margine giusto.** Pensalo per la risoluzione della lightmap: più piccola è la lightmap, più largo deve essere il margine in proporzione.
- **Le cuciture negli angoli.** Dove una isola si stacca dall'altra, la luce può fare un salto. Mettile dove non si vedono: negli angoli, sotto, dietro.
- **Muri con lo spessore, angoli chiusi.** Un muro sottile come un foglio, o due muri che non si toccano, lasciano passare la luce nel bake.
- **L'ombra dipinta.** Le cose che si muovono non hanno la lightmap. Meta consiglia di cuocere nella loro texture più dettagli che puoi, come l'occlusione ambientale: sembrano appoggiate anche senza ombre.
- **I LOD con le UV del primo.** Meta: tutti i LOD di un modello devono avere le UV della luce del LOD0, il più dettagliato.
> lavoro: 3 2

## Trucchi del mestiere · in Unity
- **Cuoci presto, cuoci spesso.** A bassa risoluzione il bake dura poco: prova spesso, e alza la risoluzione solo alla fine.
- **Scale In Lightmap.** Nel **Mesh Renderer** di un oggetto piccolo o lontano abbassalo: prende meno pixel di lightmap. A 0 l'oggetto non ha lightmap, ma fa ancora ombra agli altri.
- **Le cose minute fuori dal bake.** Unity consiglia di togliere **Contribute GI** ai detriti e agli oggetti piccoli e di illuminarli con le light probe: risparmi lightmap e tempo.
- **La memoria delle lightmap.** Le direzionali occupano circa il doppio. La compressione e **Max Lightmap Size** contano. Sul Quest le lightmap si salvano in ASTC: **Player ▸ Other Settings ▸ Lightmap Encoding**.
- **Lightmap diverse spezzano lo static batching.** Meta: gli oggetti uniti devono stare sulla stessa lightmap.
- **Ciò che si muove resta vivo, e senza ombre.** Una lampada che si accende con un interruttore, una torcia, un fascio che gira: in tempo reale, ma senza ombre, se puoi.
- **I Rendering Layers.** Una luce in tempo reale può illuminare solo certi oggetti: si sceglie nella luce e nel **Mesh Renderer**.
- **La stanza buia della lezione 3 non serve più.** Con la luce cotta una stanza chiusa è buia da sola.
- **Hai cambiato il cielo? Ricuoci.** Da Unity 6 la luce del cielo si aggiorna solo col bake.
- **La misura vera è sul visore.** Fai la build e guarda: le ombre che prima non c'erano, ora ci sono.
> lavoro: 3 2

# Da soli · Verifica · L'officina

## L'officina · apri
Tre stanze, tre scene, tre cartelli. Per ogni stanza il giro è quello del soggiorno:
1. apri la scena, e in Play sul **Display 2** leggi **Batches** e **Shadow casters**;
2. la foto del prima;
3. la cura, fuori dal Play: gli statici, le luci, le impostazioni del bake, il bake;
4. di nuovo in Play i numeri, e la foto del dopo.
Le regole del curatore valgono sempre. Non spegnere, non cancellare e non spostare lampade, mobili o oggetti: un faro al buio non è un faro ottimizzato. L'intensità e il colore delle luci li puoi cambiare.
Se hai fatto la cura e un compito resta **Da fare**, nella stanza qualcosa è stato spostato o spento: annulla con **Ctrl+Z**, o chiamami. Alla fine di ogni stanza salva con **Ctrl+S**.
Si comincia dall'officina del guardiano: tante lampade da lavoro e tanti attrezzi.
> azione: Apri l'officina | apri-scena | Lezione 08 - Luce | Officina
> verifica: scena-aperta Officina
> soluzione: Premi il pulsante qui sopra. Oppure, nella finestra **Project**, apri `Assets/Samples/RUFA Core/Lezione 08 - Luce` e fai doppio clic su `Officina`.

## L'officina · il prima
In Play, sul **Display 2**, leggi **Batches** e **Shadow casters** e scrivili nel file. Esci dal Play e fai la foto.
> azione: Foto dell'officina, prima | foto-camera | Camera Officina | Consegne/L08/officina-prima.jpg
> verifica: file-esiste Consegne/L08/officina-prima.jpg
> soluzione: Premi **Play**, nella Game view scegli **Display 2**, leggi i due numeri e scrivili nel file. Esci dal Play e premi il pulsante della foto.

## L'officina · ferma
Rendi statici la stanza e gli attrezzi piccoli, che stanno nella radice `Minuteria`: non si muovono. Le cose che si muovono restano libere.
> verifica: statici-luce Stanza | Oggetti
> soluzione: Nella Hierarchy seleziona `Stanza`, spunta **Static** e rispondi **Yes, change children**. Fai lo stesso con `Minuteria`, come con lo scaffale nel soggiorno. Non toccare `Oggetti` né `Vetri`.

## L'officina · le luci
> verifica: luci-cotte Luci
> soluzione: Nella Hierarchy apri `Luci`, seleziona tutte le luci (clic sulla prima, **Shift**+clic sull'ultima) e nel componente **Light** metti **Mode** su **Baked**.

## L'officina · cotta
Imposta il bake come nel soggiorno, e cuoci.
> attesa: 60
> verifica: luce-cotta
> soluzione: **Window ▸ Rendering ▸ Lighting**, scheda **Scene**: **Lightmap Resolution** 10, **Lightmap Compression** **Normal Quality**, **Directional Mode** **Non-Directional**. Premi **Generate Lighting** e aspetta che la barra in basso a destra sparisca.

## L'officina · la memoria
Il cartello dell'officina ha un budget in più: la memoria delle lightmap. La leggi nella finestra **Lighting**, nella riga sotto **Generate Lighting**. Gli attrezzi piccoli sono tanti, nella radice `Minuteria`, e ognuno vuole il suo pezzo di lightmap.
Rientra nel budget senza cancellare niente. Puoi abbassare la risoluzione, o togliere gli attrezzi dal bake e farli illuminare dalle probe, come consiglia Unity per gli oggetti minuti. Dopo ogni prova, ricuoci.
> verifica: lightmap-al-massimo 1 | Minuteria
> soluzione: Seleziona `Minuteria`. Nell'Inspector apri il menu accanto a **Static** e togli la spunta a **Contribute GI**: rispondi **Yes, change children**. Aggiungi un **Light Probe Group** attorno al banco e al carrello, su due strati, poi premi **Generate Lighting**. Se non basta, controlla che **Lightmap Resolution** sia 10.

## L'officina · il dopo
In Play, sul **Display 2**, rileggi i numeri e scrivili nel file, insieme alla memoria delle lightmap. Esci dal Play, fai la foto del dopo e salva.
> azione: Foto dell'officina, dopo | foto-camera | Camera Officina | Consegne/L08/officina-dopo.jpg
> verifica: file-esiste Consegne/L08/officina-dopo.jpg
> soluzione: Premi **Play**, **Display 2**, scrivi i numeri, esci dal Play e premi il pulsante della foto. **Ctrl+S**.

# Da soli · Verifica · La scala

## La scala · apri
La scala a chiocciola sale lungo la torre. Sui gradini il guardiano ha lasciato le sue cose.
> azione: Apri la scala | apri-scena | Lezione 08 - Luce | Scala
> verifica: scena-aperta Scala
> soluzione: Premi il pulsante qui sopra. Oppure, nella finestra **Project**, apri `Assets/Samples/RUFA Core/Lezione 08 - Luce` e fai doppio clic su `Scala`.

## La scala · il prima
> azione: Foto della scala, prima | foto-camera | Camera Scala | Consegne/L08/scala-prima.jpg
> verifica: file-esiste Consegne/L08/scala-prima.jpg
> soluzione: Premi **Play**, **Display 2**, leggi **Batches** e **Shadow casters** e scrivili nel file. Esci dal Play e premi il pulsante della foto.

## La scala · ferma
> verifica: statici-luce Stanza | Oggetti
> soluzione: Nella Hierarchy seleziona `Stanza`, spunta **Static** e rispondi **Yes, change children**.

## La scala · le luci
> verifica: luci-cotte Luci
> soluzione: Nella Hierarchy apri `Luci`, seleziona tutte le luci e metti **Mode** su **Baked**.

## La scala · cotta
> attesa: 60
> verifica: luce-cotta
> soluzione: Nella finestra **Lighting**: **Lightmap Resolution** 10, **Normal Quality**, **Non-Directional**, poi **Generate Lighting**.

## La scala · le probe
Le cose sui gradini sono al buio. Mettile dentro una gabbia di light probe che sale con la scala: qui lo strumento non sa fare da solo, devi seguire i gradini.
> attesa: 60
> verifica: probe-attorno Oggetti | 16
> soluzione: **GameObject ▸ Light ▸ Light Probe Group**. Con **Edit Light Probe Group** porta le otto probe attorno ai primi gradini, poi premi **Select All** e **Duplicate Selected**, e sposta la copia più su, lungo la salita. Ripeti finché ogni cosa sui gradini ha probe attorno, sopra e sotto. Poi **Generate Lighting**.

## La scala · il dopo
> azione: Foto della scala, dopo | foto-camera | Camera Scala | Consegne/L08/scala-dopo.jpg
> verifica: file-esiste Consegne/L08/scala-dopo.jpg
> soluzione: Premi **Play**, **Display 2**, scrivi i numeri, esci dal Play e premi il pulsante della foto. **Ctrl+S**.

# Da soli · Verifica · La lanterna

## La lanterna · apri
In cima alla torre c'è la lanterna: la stanza di vetro con la lente. Il fascio gira, quindi non si può cuocere: una luce cotta resterebbe ferma mentre la lampada gira.
> azione: Apri la lanterna | apri-scena | Lezione 08 - Luce | Lanterna
> verifica: scena-aperta Lanterna
> soluzione: Premi il pulsante qui sopra. Oppure, nella finestra **Project**, apri `Assets/Samples/RUFA Core/Lezione 08 - Luce` e fai doppio clic su `Lanterna`.

## La lanterna · il prima
> azione: Foto della lanterna, prima | foto-camera | Camera Lanterna | Consegne/L08/lanterna-prima.jpg
> verifica: file-esiste Consegne/L08/lanterna-prima.jpg
> soluzione: Premi **Play**, **Display 2**, leggi **Batches** e **Shadow casters** e scrivili nel file. Esci dal Play e premi il pulsante della foto.

## La lanterna · ferma
> verifica: statici-luce Stanza | Oggetti
> soluzione: Nella Hierarchy seleziona `Stanza`, spunta **Static** e rispondi **Yes, change children**.

## La lanterna · le luci
Tutte cotte, tranne il fascio.
> verifica: luci-cotte Luci | Fascio
> soluzione: Nella Hierarchy apri `Luci` e seleziona tutte le luci tranne `Fascio`: tieni premuto **Ctrl** e clicca le altre una per una. Metti **Mode** su **Baked**.

## La lanterna · il fascio
Il fascio resta in tempo reale. Ma le sue ombre? Sul visore le luci aggiuntive non fanno ombra comunque: toglierle non cambia niente nel Quest, e sul PC risparmi.
> verifica: luce-viva Fascio
> soluzione: Seleziona `Fascio`. Nel componente **Light** lascia **Mode** su **Realtime** e metti **Shadow Type** su **No Shadows**.

## La lanterna · cotta
> attesa: 60
> verifica: luce-cotta
> soluzione: Nella finestra **Lighting**: **Lightmap Resolution** 10, **Normal Quality**, **Non-Directional**, poi **Generate Lighting**.

## La lanterna · i riflessi
L'ottone riflette il cielo anche dove dovrebbe riflettere la stanza.
> attesa: 60
> verifica: riflesso-cotto Stanza
> soluzione: **GameObject ▸ Light ▸ Reflection Probe**. **Position** `0`, `1.6`, `0`; **Box Size** `5.4`, `3.2`, `5.4`; spunta **Box Projection**; **Blend Distance** a `0.1`. Poi **Generate Lighting**.

## La lanterna · il dopo
> azione: Foto della lanterna, dopo | foto-camera | Camera Lanterna | Consegne/L08/lanterna-dopo.jpg
> verifica: file-esiste Consegne/L08/lanterna-dopo.jpg
> soluzione: Premi **Play**, **Display 2**, scrivi i numeri, esci dal Play e premi il pulsante della foto. **Ctrl+S**.

## Il curatore
Come nel soggiorno: il compagno fa il curatore sulle tue foto delle tre stanze, e tu sulle sue. Per ogni stanza sceglie la foto che metterebbe nel gioco, e dice perché. Se ti dice che una stanza ha perso l'atmosfera, torna lì e regola le lampade. Scrivi nel file delle misure cosa ha scelto e cosa hai cambiato.
> lavoro: 4 3

# Da soli · Il tuo progetto

## La tua scena
Ora il tuo progetto. Premi il pulsante: si apre la scena del tuo blockout.
> azione: Apri la tua scena | crea-progetto
> verifica: scena-aperta Progetto

## La camera
Nelle lezioni 6 e 7 hai messo nella tua scena una camera fissa, `Camera foto`, sul **Display 2**. Usa quella. Se non c'è, rifalla:
1. Nella Scene view inquadra il punto che conta di più, all'altezza degli occhi.
2. **GameObject ▸ Camera**, e rinominala `Camera foto`.
3. Con la camera selezionata, **GameObject ▸ Align With View**.
4. Nell'Inspector della camera metti **Target Display** su **Display 2**.
> verifica: scena-componente Camera foto | Camera
> lavoro: 3 1

## Il prima
1. Premi **Play**, scegli **Display 2** e leggi **Batches** e **Shadow casters**. Nel Light Explorer conta le luci in tempo reale.
2. Scrivi tutto nel file delle misure, nella parte del progetto. Esci dal Play.
3. Fai la foto.
> azione: Foto del progetto, prima | foto-camera | Camera foto | Consegne/L08/progetto-prima.jpg
> verifica: file-esiste Consegne/L08/progetto-prima.jpg

## La cura
Cuoci la luce della tua scena, come nel faro:
- statici i muri, il pavimento e i mobili fermi; non i mattoncini che si muovono: porte, leve, cassetti, oggetti da prendere;
- le luci in **Baked**, tranne quelle che si muovono o che un interruttore accende e spegne: quelle restano **Realtime**, senza ombre se puoi;
- le superfici che brillano con **Global Illumination** su **Baked**;
- le light probe dove si cammina e dove si muovono le cose, su due strati;
- una reflection probe per stanza, con **Box Projection** e **Blend Distance** a `0.1`;
- nella finestra **Lighting**, i valori del soggiorno, e **Generate Lighting**.
Se vuoi fare prima, **RUFA ▸ Strumenti** ha le scorciatoie: il preset della luce per il visore, una griglia di probe sull'oggetto selezionato, una reflection probe sull'oggetto selezionato.
Scrivi nel file cosa hai cotto, cosa hai lasciato in tempo reale e perché.
> lavoro: 25 15

## Cotta
Questa pagina controlla che la tua scena abbia la sua luce cotta, e che ogni oggetto statico abbia la sua lightmap.
> verifica: luce-cotta
> lavoro: 1 1

## Il dopo
1. In Play, sul **Display 2**, rileggi i numeri e scrivili nel file. Esci dal Play.
2. Fai la foto del dopo.
3. Salva la scena con **Ctrl+S**.
Se hai tempo, fai la build per il visore e guarda la tua scena col Quest: le ombre ci sono.
> azione: Foto del progetto, dopo | foto-camera | Camera foto | Consegne/L08/progetto-dopo.jpg
> verifica: file-esiste Consegne/L08/progetto-dopo.jpg

## Le misure
Controlla che nel file delle misure non resti nessun «da scrivere».
> verifica: file-compilato Consegne/L08/misure.md | «da scrivere»
> lavoro: 5 3

## A fine lezione
Salva il lavoro. Nella cartella della tua copia lancia:
`git salva "Lezione 08"`
