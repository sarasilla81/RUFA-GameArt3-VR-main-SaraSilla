# Lezione 8 — La luce cotta nel faro: la guida

Questa è la guida da tenere aperta mentre lavori. Ci ho messo le stesse procedure del tutorial, tutte in un posto solo e in ordine, il laboratorio di Blender passo per passo e i trucchi del mestiere. E ci ho messo tutte le regole che Meta e Unity danno sulla luce in VR. Ogni regola ha il suo link, e in fondo trovi l'elenco completo delle fonti.

## 1. Il cliente e il budget
Oggi lavori per una società di escape room in realtà virtuale. Ha restaurato il vecchio faro dell'isola e lo vuole sul Quest 3S, di sera, con le lampade accese. I mobili e gli oggetti vengono quasi tutti da Poly Haven, dalla collezione «Project Lighthouse», fatta proprio per la casa del guardiano di un faro. Sono modelli CC0, cioè liberi: si possono usare e cambiare senza chiedere il permesso.

Il problema di oggi non sono i triangoli, e nemmeno i materiali. È la luce.

**La luce in tempo reale.** Una luce in tempo reale si calcola a ogni frame, su ogni pixel che tocca. Unity lo scrive chiaro: calcolare le luci mentre il gioco gira può costare, soprattutto nelle scene complesse o sull'hardware poco potente ([Unity, i modi della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/light-modes/introduction)). Il visore è un dispositivo mobile, e in VR i pixel sono tanti: il nostro progetto chiede 90 frame al secondo, e ogni frame si disegna per due occhi.

Nel faro trovi tre tipi di luce. La **Directional** è il sole: arriva da lontano, coi raggi tutti paralleli. La **Spot** è un faretto, fa luce a cono. La **Point** è una lampadina, fa luce in tutte le direzioni. Quasi tutte le lampade del faro sono luci Point.

**Le ombre.** Per fare un'ombra, la GPU prima disegna la scena dal punto di vista della luce. Ne esce un'immagine, la **mappa delle ombre** (in inglese shadow map), che dice cosa vede la luce. Quello che la luce non vede è in ombra. Quindi ogni oggetto che fa ombra si disegna una volta in più, per ogni luce con le ombre. Meta lo dice così: ogni mesh che fa o riceve ombra richiede una draw call in più, disegnata dal punto di vista della luce ([Meta, RenderDoc](https://developers.meta.com/horizon/documentation/unity/po-renderdoc-optimizations-1/)).

Una luce Spot guarda da una parte sola, e le basta una mappa. Una luce Point guarda dappertutto. Unity scrive che per la sua mappa delle ombre fotografa la scena in sei direzioni, e che costa come le ombre di sei luci Spot. Sui dispositivi mobili questo si prende una parte importante delle risorse di ogni frame ([Unity, ottimizzare le ombre in URP](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/shadows-in-urp/shadows-optimization)). Unity fa anche il conto: quattro luci Spot e una Point fanno dieci mappe delle ombre, una per ogni Spot e sei per la Point ([Unity, la risoluzione delle ombre in URP](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/shadows-in-urp/shadow-resolution-urp)).

Conta anche quanti oggetti fanno ombra. Unity: il numero di oggetti che fanno ombra in tempo reale pesa molto sulle prestazioni delle ombre ([Unity, ottimizzare le ombre in URP](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/shadows-in-urp/shadows-optimization)). E Meta: le ombre dinamiche, cioè calcolate mentre il gioco gira, possono avere un impatto enorme sulle prestazioni. Si calcolano a ogni frame, e caricano molto sia la CPU sia la GPU ([Meta, Art Direction for All-in-One VR Performance](https://developers.meta.com/horizon/documentation/unity/po-art-direction/)).

**Quante luci regge URP.** Col percorso Forward, URP accende al massimo 9 luci su ogni oggetto: una luce principale, di solito il sole, e 8 luci aggiuntive. Per ogni camera, sui dispositivi mobili, le luci aggiuntive sono al massimo 32 ([Unity, i limiti delle luci in URP](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/light-limits-in-urp)). Nell'asset di URP il cursore **Per Object Limit** decide quante luci aggiuntive possono toccare ogni oggetto: nel nostro progetto 4. Le altre, quell'oggetto non lo illuminano. Col Forward+, che il nostro progetto usa sul PC, Unity ignora questo limite; sul visore, che usa il Forward, vale ([Unity, l'asset di URP](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/urp-reference/universalrp-asset)). Sono limiti tecnici, non un budget.

**Sul visore le lampade non fanno ombra.** Il nostro progetto ha due profili di URP: `PC_RPAsset`, che l'editor usa per il PC, e `Mobile_RPAsset`, per il visore. Nel profilo del visore le luci aggiuntive non fanno ombra. È un consiglio di Unity: nella parte **Additional Lights** dell'asset spegni **Cast Shadows**, e risparmi lavoro della GPU e memoria ([Unity, configurare URP per le prestazioni](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/introduction/configure-for-better-performance)). Le luci aggiuntive sono tutte, tranne la principale. Quindi sul visore le lampade del faro non fanno nessuna ombra. L'ombra della sedia sotto la lampada, il buio dietro la stufa: in tempo reale, sul visore, non ci sono. Fa ombra solo il sole.

Le ombre delle lampade te le ridà la luce cotta. Il bake calcola le ombre una volta sola, nell'editor, e le scrive in un'immagine. Sul visore si vedono, morbide, e mentre il gioco gira non costano niente.

Per vederlo, senza cambiare niente:
1. Nella finestra **Project** apri `Assets/Settings` e seleziona `Mobile_RPAsset`.
2. Nell'Inspector apri il riquadro **Lighting**. Sotto **Additional Lights** la casella **Cast Shadows** è spenta.
3. Lasciala così: questo asset decide come disegna il visore, in tutto il progetto.

**Il budget del cliente.** Il cliente ha messo un cartello in ogni stanza, col budget: quante **Batches** al massimo, e quante luci in tempo reale. Nel soggiorno, nell'officina e nella scala, nessuna luce in tempo reale. Nella lanterna una sola: il fascio del faro, che gira. Nell'officina il cartello dice anche quanta memoria possono occupare le lightmap, le immagini della luce cotta.

Non è un ripiego da studenti. Per il Quest, lo studio Camouflaj ha cotto la luce in tutto Batman: Arkham Shadow, per abbassare i costi ([Meta, la luce di Batman: Arkham Shadow](https://developers.meta.com/horizon/blog/gdc-2025-design-strategies-considerations-tips-mr-vr-horizon-os/)). E Meta racconta un'esperienza fatta sul primo Quest. Con la sola luce cotta reggeva più di un milione di triangoli. Con la luce cotta più una luce direzionale in tempo reale, le normal map e le mappe dei riflessi, si fermava intorno ai 300.000 ([Meta, Open World Games and Asset Streaming](https://developers.meta.com/horizon/documentation/unity/po-assetstreaming/)).

**Le regole di Meta.** Prima un termine. La **GI**, l'illuminazione globale, è la luce completa: quella che arriva dalle lampade, più quella che rimbalza da una superficie all'altra.
- [Meta, Lighting and effects](https://developers.meta.com/horizon/design/lighting-and-effects/): cuoci la luce ogni volta che puoi. Usa le luci in tempo reale con parsimonia, perché consumano molte risorse.
- [Meta, Android Development](https://developers.meta.com/horizon/documentation/unity/unity-mobile-performance-intro/): preferisci la geometria statica, con le lightmap. Usa sempre le lightmap cotte, non la GI precalcolata. In queste regole, scritte per la pipeline vecchia di Unity, Meta arriva a dire: niente luce dinamica.
- [Meta, Best practices for VR](https://developers.meta.com/horizon/documentation/unity/unity-best-practices-intro/): evita la GI in tempo reale. Evita troppe luci calcolate per pixel, oppure usa il Forward+, che velocizza i calcoli della luce. Spegni le ombre quando ti avvicini al limite dei triangoli o delle draw call.
- [Meta, Art Direction for All-in-One VR Performance](https://developers.meta.com/horizon/documentation/unity/po-art-direction/) e [Meta, Balancing art and performance](https://developers.meta.com/horizon/design/art-and-performance/): al posto delle ombre dinamiche, usa la luce statica dell'ambiente. Si cuoce nelle lightmap, nelle light probe, o a volte perfino nella texture di un oggetto fermo, e il guadagno è grande.
- [Meta, RenderDoc](https://developers.meta.com/horizon/documentation/unity/po-renderdoc-optimizations-1/): se le ombre ti servono davvero, tieni più basse possibile la loro risoluzione e le draw call in più. Controlla la risoluzione totale delle mappe delle ombre.
- [Meta, Minimize Iteration Time](https://developers.meta.com/horizon/resources/unity-settings-features-minimize-iteration/): non accendere di base le impostazioni che disegnano la scena più volte, come le ombre.
- [Meta, Forward+ Rendering](https://developers.meta.com/horizon/documentation/unity/unity-forward-plus-rendering/): il Forward+ conviene con tante luci in tempo reale. Nelle prove di Meta comincia a fare meglio del Forward intorno alle 5 luci. Con poche luci, o con mesh molto piccole, Meta consiglia di restare sul Forward.

**Le regole di Unity.**
- [Unity, ottimizzare le ombre in URP](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/shadows-in-urp/shadows-optimization): sui dispositivi mobili, poche luci Point con le ombre, o nessuna.
- [Unity, configurare URP per le prestazioni](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/introduction/configure-for-better-performance): col Forward, **Additional Lights** su **Disabled**, o su **Per Vertex**: le luci aggiuntive spente, o calcolate sui vertici invece che su ogni pixel. Le ombre delle luci aggiuntive spente. Nelle ombre, **Max Distance** più corta, così meno oggetti entrano nella passata delle ombre, e meno cascate in **Cascade Count**. Le ombre morbide spente, o con una qualità più bassa.
- [Unity, l'asset di URP](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/urp-reference/universalrp-asset): le cascate valgono solo per l'ombra del sole, e più sono, più costano. Le ombre morbide pesano molto sulle piattaforme mobili e sui visori autonomi.
- [Unity, i percorsi di disegno](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/introduction/urp-concepts/rendering-paths/comparison): usa il Forward se hai poche luci, o se disegni per un dispositivo mobile.

Quante luci in tempo reale può avere una scena sul Quest? Né Meta né Unity danno un numero. Ne parliamo nella sezione 11.

## 2. Il faro
**I modelli nuovi.** La prima pagina del tutorial controlla che nel progetto ci siano i modelli del faro, in `Assets/Demo/Models/Faro`. Se resta **Da fare**, chiudi Unity, lancia `git aggiorna` un'altra volta nella cartella della tua copia e riapri il progetto. Unity Hub deve essere aperto, perché il progetto riscarica i suoi package.

**Il rig del PC.** Oggi il visore resta spento. Dal menu **RUFA ▸ Play mode**, se accanto a **Simulatore VR** c'è la spunta, clicca la voce: la spunta sparisce. In Play clicca nella Game view per guardare col mouse e cammina con **W A S D**. **Esc** ti ridà il mouse. Le cose che si muovono si prendono in mano: guardale col mirino e tieni premuto il tasto sinistro del mouse. Quando lo lasci, cadono.

**L'editor sul PC.** I budget di oggi sono pensati per l'editor impostato per il PC. Se l'ultima volta hai fatto una build per il visore, l'editor è rimasto su Android: disegna come il visore, e i numeri vengono diversi. Dal menu **File ▸ Build Profiles**, a sinistra scegli **Windows** e premi **Switch Platform**. Unity ci mette qualche minuto.

**Apri il faro.** Il pulsante **Apri il faro** del tutorial copia il faro in `Assets/Samples/RUFA Core/Lezione 08 - Luce` e apre la scena `Soggiorno`. La copia è tua: se la rovini, cancella quella cartella e premi di nuovo il pulsante.

Il faro ha quattro scene, una per stanza: `Soggiorno`, `Officina`, `Scala` e `Lanterna`. Il motivo è il bake: Unity cuoce tutta la scena aperta. Con una scena per stanza il bake dura poco, e quando cuoci una stanza non cambi le foto del prima delle altre.

**Le stanze.**
- **Il soggiorno del guardiano**, che curiamo insieme e dove fai il laboratorio. Sei metri per cinque, i muri intonacati, le travi, il pavimento di legno, una finestra sul mare. Il tavolo con la tovaglia e le sedie, la stufa con la brace accesa, la radio col quadrante acceso, il divanetto, l'orologio, il salvagente, lo specchio, un quadro e lo scaffale del laboratorio. Le luci: la lanterna appesa sopra il tavolo, la lampada a olio, due applique, cioè lampade da muro, e il sole del tramonto. Le cose che si muovono: una torcia, un binocolo, una bussola, un orologio da tasca, un bollitore, una brocca.
- **L'officina**, sette metri per cinque. Il banco con la morsa, il carrello, la cassettiera, il trapano a colonna, gli scaffali di metallo con taniche, latte e cassette degli attrezzi, e decine di attrezzi piccoli. Le luci: tre neon, due lampade da muro, la lampada da banco e una lampada appesa sopra il carrello.
- **La scala a chiocciola**: la torre vista dal basso. I gradini girano attorno alla colonna, e nel muro ci sono le feritoie, le finestre strette. Lungo la salita ci sono le applique. Sui gradini qualcuno ha lasciato una lanterna, una tanica, gli stivali, il cappello e una torcia.
- **La lanterna**, la stanza di vetro in cima al faro. Al centro la lente, nella sua gabbia d'ottone, e il fascio di luce che gira. Intorno la ringhiera, il tavolino del guardiano col registro e la lampada, e il sole del tramonto.

**Come sono fatte.** In ogni scena le cose stanno in poche radici, gli oggetti in cima alla Hierarchy:
- `Stanza`: i muri, il pavimento, i mobili che non si muovono e i corpi delle lampade. È quello che renderai statico.
- `Luci`: tutte le luci. All'inizio sono tutte **Realtime**, con le ombre morbide, **Soft Shadows**.
- `Oggetti`: le cose che si muovono. In Play le prendi in mano. Non vanno mai rese statiche.
- `Vetri`: i vetri delle finestre. Nemmeno loro vanno statici: nel bake fermerebbero la luce del sole.
- nel soggiorno, `Laboratorio`: lo scaffale del laboratorio, in una radice sua;
- nell'officina, `Minuteria`: gli attrezzi piccoli.

I muri e i pavimenti sono fatti di cubi e cilindri di Unity, che hanno già le UV della luce: te le spiego nella sezione 4. I mobili e le lampade sono modelli importati con **Generate Lightmap UVs** acceso, tranne lo scaffale: quello lo sistemi tu nel laboratorio.

**Le camere.** Ogni stanza ha una camera fissa sul **Display 2**: `Camera Soggiorno`, `Camera Officina`, `Camera Scala`, `Camera Lanterna`. Nel soggiorno ce n'è una seconda, `Camera Scaffale`, sul **Display 3**: inquadra lo scaffale. Le camere hanno il tag `EditorOnly`: nell'app per il visore non ci sono.
1. Clicca la linguetta **Game**.
2. In alto a sinistra nella Game view apri il menu **Display 1** e scegli **Display 2**. Il **Display 1** è il tuo rig, in Play.
3. Nel menu delle proporzioni, quello con scritto **Free Aspect**, scegli **16:9 Aspect**. Così l'inquadratura è la stessa su tutti i PC.
4. Premi **Stats** in alto a destra: compare il riquadro coi numeri del frame.

**Il cartello.** Ogni stanza ha il suo cartello, col nome e il budget del cliente.

**Il tuo quadro.** Nel soggiorno c'è un quadro con la tela bianca. È per te: se hai un tuo disegno o una tua grafica, mettila lì. Trascina il file PNG o JPG nella cartella `Assets/Progetto`. Poi nella casella di ricerca della Hierarchy scrivi `Tela` e seleziona l'oggetto. Apri il suo materiale in fondo all'Inspector e trascina l'immagine sul quadratino accanto a **Base Map**. Poi svuota la ricerca con la crocetta.

**Il file delle misure.** Il pulsante del tutorial crea `Consegne/L08/misure.md` e lo apre nel blocco note. Tienilo aperto: ci scrivi i numeri di ogni stanza, prima e dopo.

## 3. Come si misura
**Cosa conta Stats.** Oggi nel riquadro **Stats** guardi due numeri ([Unity, la finestra Statistics](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/profiling/rendering-statistics)):
- **Batches**: le draw call del frame, come nella lezione 7. Stats conta ogni passata, anche quelle delle ombre.
- **Shadow casters**: gli oggetti del frame che fanno ombra. Ogni luce con le ombre li ridisegna nella sua mappa delle ombre, e una luce Point sei volte. Per questo, in una stanza piena di lampade, le **Batches** salgono tanto.

**Si misura in Play.** Come nella lezione 7: lo static batching parte in Play, e la spunta **Static** che darai alla stanza accende anche quello. Quindi misuri in Play, e curi fuori dal Play. Quello che cambi in Play sparisce quando esci.
1. Premi **Play**.
2. Nella Game view scegli il display della stanza. Se il menu non risponde, premi **Esc** per riavere il mouse.
3. Leggi il numero di **Batches** e scrivilo nel file delle misure, alla riga della stanza. Guarda anche **Shadow casters**.
4. Confronta **Batches** col budget sul cartello.
5. Premi di nuovo **Play** per uscire.

**Le luci in tempo reale: il Light Explorer.** Il **Light Explorer** è una finestra che elenca tutto quello che fa luce nella scena: le luci, le reflection probe, le light probe e i materiali emissivi degli oggetti statici ([Unity, il Light Explorer](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/lighting-explorer/lighting-explorer)).
1. Dal menu in alto apri **Window ▸ Rendering ▸ Light Explorer**.
2. Nella prima scheda, quella delle luci, ogni riga è una luce. La colonna **Mode** dice se è **Realtime**, **Mixed** o **Baked**.
3. Conta le **Realtime** e le **Mixed**: le Mixed costano come quelle in tempo reale. Scrivi il numero nel file, accanto alle **Batches**.

**Le passate delle ombre: il Frame Debugger.** Stats ti dice quanti oggetti fanno ombra. Il **Frame Debugger** ti fa vedere i disegni uno per uno ([Unity, il Frame Debugger](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/profile-rendering/frame-debugger/debug)).
1. Premi **Play** e scegli il display della stanza.
2. Dal menu in alto apri **Window ▸ Analysis ▸ Frame Debugger** e premi **Enable**. Il gioco si ferma su un frame.
3. Nell'elenco a sinistra, verso l'alto, ci sono le passate delle ombre: **Draw Main Light Shadowmap**, per il sole, e **Draw Additional Lights Shadowmap**, per le lampade. Apri quelle righe col triangolino. Ogni riga dentro è un disegno fatto solo per l'ombra.
4. Trascina il cursore in alto nel Frame Debugger: la Game view disegna il frame fino a quel punto, un disegno alla volta.
5. Premi **Disable** ed esci dal Play.

Dopo la cura, quelle passate spariscono.

**La memoria delle lightmap.** Una lightmap è un'immagine, e le immagini occupano memoria. Dopo il bake, la finestra **Lighting** lo scrive in fondo, sotto il pulsante **Generate Lighting**: quante lightmap ha fatto, quanto sono grandi in pixel e quanti MB occupano. Nell'officina il cartello ha un budget anche per questo numero.

**Il profilo giusto.** I budget dei cartelli sono pensati per l'editor impostato per il PC. Con l'editor su Android i numeri cambiano: Unity disegna col profilo del visore, le lampade non fanno ombra e manca la passata dell'SSAO della lezione 7. Misura sempre con l'editor sul PC: la pagina «L'editor sul PC» del tutorial lo controlla.

**Sul visore.** Stats sul PC conta le passate del PC. I numeri veri del Quest li leggi col Profiler collegato al visore, nel modulo **Rendering** ([Unity, il modulo Rendering](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/profile-rendering/profiler-rendering)), e con OVR Metrics, come nella lezione 5. Unity lo ripete: misura sempre sulla piattaforma di destinazione ([Unity, ridurre il lavoro di CPU e GPU](https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/optimizing-graphics-performance)).

## 4. Cuocere la luce
**Cuocere.** Cuocere la luce, in inglese **bake**, vuol dire calcolarla una volta sola, nell'editor, prima del gioco. Unity scrive il risultato in immagini che si chiamano **lightmap**, mappe della luce. Si posano sugli oggetti fermi come una texture. Mentre il gioco gira, la luce cotta non si calcola più. Unity: per le luci cotte non c'è nessun costo mentre il gioco gira; applicare le lightmap alla scena costa poco ([Unity, i modi della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/light-modes/introduction)).

**Diretta e indiretta.** La luce **diretta** va dalla lampada alla superficie. La luce **indiretta** è quella che rimbalza: il muro chiaro che illumina il soffitto, il tappeto rosso che tinge di rosso il muro accanto. Una luce in tempo reale, di base, dà solo la luce diretta. Unity lo dice: le ombre vengono nere del tutto, e manca la luce che rimbalza, come il colore che passa da una superficie all'altra (stessa pagina). Il bake invece cuoce tutte e due.

**I tre modi della luce.** Ogni luce ha un **Mode**, nel componente **Light**. Unity confronta i tre modi così ([Unity, scegliere il modo della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/light-modes/choose)):

| Mode | Quanto costa mentre il gioco gira | Quanto aspetti per vedere un cambio |
|---|---|---|
| **Realtime** | il costo più alto | poco |
| **Mixed** | alto | tanto |
| **Baked** | niente | più di tutti |

Nel dettaglio ([Unity, i modi della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/light-modes/introduction)):
- **Realtime**: Unity la calcola mentre il gioco gira, una volta per frame. Fa ombra fino a una distanza massima, che in URP si regola nell'asset, alla voce **Max Distance**.
- **Mixed**: un po' cotta e un po' in tempo reale. Unity avverte che costa come una luce in tempo reale. Rispetto alla luce tutta cotta fa sempre più calcoli mentre il gioco gira; rispetto a quella tutta in tempo reale occupa più memoria. Come si comporta dipende dal **Lighting Mode**, qui sotto.
- **Baked**: Unity cuoce la sua luce, diretta e indiretta, nelle lightmap e nelle light probe. Mentre il gioco gira non la puoi più cambiare. Non fa il **riflesso speculare**, il punto lucido che una lampada accende su una superficie liscia. E non illumina le cose che si muovono: quelle prendono la sua luce solo dalle light probe, nella sezione 5.

Unity aggiunge un dettaglio da sapere: se non usi nessun sistema di GI, o solo quello in tempo reale, le luci Baked e Mixed si comportano come se fossero Realtime.

**Il Lighting Mode.** Conta solo per le luci Mixed, e vale per tutta la scena. Sta nelle impostazioni della luce della scena, sotto **Mixed Lighting** ([Unity, impostare il Lighting Mode](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/lighting-mode/light-mode-scene)). Unity ne elenca quattro, dal più fedele al più semplice ([Unity, il Lighting Mode](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/lighting-mode/lighting-mode)):
- **Distance Shadowmask**: la luce più realistica anche lontano, e il costo più alto. Unity lo propone per i mondi aperti, sulle piattaforme potenti o medie.
- **Shadowmask**: come il precedente, ma con meno ombre in tempo reale e meno costo. Le ombre cotte degli oggetti fermi stanno in texture apposta, con al massimo quattro luci Mixed sovrapposte nello stesso punto. Se se ne sovrappongono di più, Unity cuoce del tutto quelle in più ([Unity, ottimizzare le ombre in URP](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/shadows-in-urp/shadows-optimization)).
- **Baked Indirect**: la luce diretta in tempo reale, quella che rimbalza cotta, le ombre in tempo reale.
- **Subtractive**: una luce semplice e ombre meno fedeli, ma un costo basso. Unity lo propone per l'hardware poco potente, le scene semplici con poche luci, gli stili non realistici. Sugli oggetti fermi la luce diretta è cotta; quelli che si muovono la prendono in tempo reale, e fanno ombra solo con la luce direzionale più forte.

Per i dispositivi mobili di fascia media e bassa Unity indica lo **Shadowmask**, insieme alle light probe ([Unity, ottimizzare le ombre in URP](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/shadows-in-urp/shadows-optimization)). Meta racconta una scelta diversa. In Batman: Arkham Shadow, Camouflaj ha cotto la luce dappertutto, e dove serviva la luce mista ha usato il **Subtractive**: la luce diretta nella lightmap per gli oggetti fermi, in tempo reale per quelli che si muovono; la luce che rimbalza nella lightmap, e per le cose che si muovono nelle light probe ([Meta, la luce di Batman: Arkham Shadow](https://developers.meta.com/horizon/blog/gdc-2025-design-strategies-considerations-tips-mr-vr-horizon-os/)).

Nel faro le luci diventano tutte **Baked**: il Lighting Mode non le tocca.

**Gli statici.** La lightmap la ricevono solo gli oggetti statici. Nell'Inspector, in alto a destra, c'è la casella **Static** con una freccia ([Unity, gli oggetti statici](https://docs.unity.com/en-us/engine/6000.3/manual/working-with-gameobjects/gameobject-fundamentals/static-objects)). La spunta accende tutti i flag insieme; la freccia apre l'elenco, per sceglierli uno a uno. Per la luce ne contano due:
- **Contribute GI**: l'oggetto entra nei calcoli della luce, durante il bake.
- **Reflection Probe Static**: l'oggetto si vede nei riflessi delle reflection probe cotte, nella sezione 6.

Con **Contribute GI** acceso, nel componente **Mesh Renderer**, sotto **Lighting**, si sblocca **Receive Global Illumination**. Decide da dove l'oggetto prende la luce cotta: dalle **Lightmaps** o dalle **Light Probes** ([Unity, il Mesh Renderer](https://docs.unity.com/en-us/engine/6000.3/manual/assets-and-media/asset-types/mesh/components-reference/class-mesh-renderer)). Per i muri e i mobili resta su **Lightmaps**.

La regola è quella della lezione 7: statico solo ciò che è fermo. Una cosa che si prende in mano, una porta, un cassetto: mai statici.

**Le UV della luce.** Le **UV** sono il modo in cui la superficie di un modello si stende sul piano, come un vestito scucito, per posarci sopra un'immagine. La lightmap ha bisogno di UV sue: ogni faccia deve avere il suo posto, senza sovrapporsi alle altre. Unity le cerca nel secondo canale delle UV della mesh. Nel codice si chiama `Mesh.uv2`, nel manuale «UV1»: due nomi per la stessa cosa ([Unity, generare le UV della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/lighting-gi-uvs/generating-lightmapping-uvs)).

Per i modelli importati Unity le sa fare da solo (stessa pagina):
1. Seleziona il modello nella finestra **Project**.
2. Nell'Inspector apri la scheda **Model**, sezione **Geometry**, e spunta **Generate Lightmap UVs**.
3. Premi **Apply**. Unity scrive le UV della luce nel secondo canale.

I cubi e i cilindri di Unity le hanno già. I mobili del faro sono stati importati con **Generate Lightmap UVs** acceso. Lo scaffale no: le sue le rifai a mano nel laboratorio, nella sezione 7.

**La finestra Lighting.** Le impostazioni del bake stanno nella finestra **Lighting**: dal menu **Window ▸ Rendering ▸ Lighting** ([Unity, la finestra Lighting](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-reference/lighting-window)). In alto ha cinque schede: **Scene**, **Adaptive Probe Volumes**, **Environment**, **Realtime Lightmaps** e **Baked Lightmaps**. Oggi lavori nella scheda **Scene**.

Nella scheda **Scene** il campo **Lighting Settings Asset** dice quale asset di impostazioni usa la scena. Ogni stanza del faro ha il suo, col nome della stanza, e parte coi valori che Unity dà a un asset nuovo. Se una scena non ne ha uno, il pulsante **New Lighting Settings** lo crea e lo assegna alla scena ([Unity, l'asset Lighting Settings](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-reference/class-lighting-settings)).

Le voci da conoscere, una per una:
- **Realtime Global Illumination**: spento. È la GI calcolata mentre il gioco gira, il sistema che Unity chiama Enlighten, e Meta chiede di evitarla ([Meta, Best practices for VR](https://developers.meta.com/horizon/documentation/unity/unity-best-practices-intro/)). In URP, Unity scrive che il suo supporto finisce dopo Unity 6 ([Unity, il confronto fra le pipeline](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/choose-a-render-pipeline/feature-comparison)).
- **Baked Global Illumination**: acceso. È il sistema che cuoce la luce nelle lightmap. Sta sotto **Mixed Lighting**, insieme al **Lighting Mode**.
- **Lightmapper**, sotto **Lightmapping Settings**: chi fa il calcolo. **Progressive GPU** usa la scheda video, **Progressive CPU** il processore ([Unity, CPU o GPU](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/configure/progressive-lightmapper)). Unity scrive che nella maggior parte dei casi la scheda video è molto più veloce ([Unity, ottimizzare il bake](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/gpuprogressive-lightmapper)). In Unity 6.3 un asset nuovo parte già con la scheda video ([Unity, le novità della 6.3](https://docs.unity.com/en-us/engine/6000.3/manual/whats-new/unity63)).
- **Lightmap Resolution**: quanti **texel** per unità. Un texel è un pixel della lightmap; un'unità di Unity è un metro. Unity avverte: se raddoppi la risoluzione, i texel diventano quattro volte tanti ([Unity, le impostazioni del bake](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/lightmaps-reference)).
- **Max Lightmap Size**: la grandezza massima di ogni lightmap, in pixel. Il valore di Unity è 1024 (stessa pagina). Se la luce della stanza non ci sta, Unity fa più lightmap.
- **Lightmap Compression**: quanto Unity comprime le lightmap. Le scelte sono **None**, **Low Quality**, **Normal Quality** e **High Quality**. Unity descrive **Normal Quality** come un buon compromesso fra la memoria e l'aspetto (stessa pagina).
- **Directional Mode**: **Directional** o **Non-Directional**. Una lightmap direzionale tiene anche la direzione da cui arriva la luce, e Unity scrive che occupa circa il doppio della memoria video. Quelle non direzionali occupano meno memoria e meno spazio su disco, e lo shader le legge più in fretta; in cambio l'aspetto è un po' meno ricco (stessa pagina). Nemmeno la lightmap direzionale tiene il riflesso speculare: le lightmap servono solo alla luce diffusa, quella opaca, senza riflessi ([Unity, le lightmap direzionali](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/configure-lightmapping-settings/directional)). Meta è netta: usa le lightmap non direzionali ([Meta, Android Development](https://developers.meta.com/horizon/documentation/unity/unity-mobile-performance-intro/)).
- I campioni: **Direct Samples**, **Indirect Samples** ed **Environment Samples**. Per calcolare la luce di un texel il lightmapper lancia dei raggi, i **campioni**: verso le luci, verso i rimbalzi, verso il cielo. Pochi campioni fanno una luce granulosa, col **rumore**; tanti allungano il bake. Per **Indirect Samples** Unity scrive che in certe scene, soprattutto all'aperto, 128 dovrebbero bastare; negli interni con superfici emissive va alzato finché il rumore è accettabile. **Environment Samples** parte da 256 ([Unity, le impostazioni del bake](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/lightmaps-reference)). Unity avverte anche che sulla scheda video, per la stessa qualità del processore, possono servire fino a quattro volte i campioni indiretti ([Unity, ottimizzare il bake](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/gpuprogressive-lightmapper)).
- **Max Bounces**: quante volte la luce rimbalza. Unity parte da 2, e scrive che fino a 10 vanno bene per quasi tutte le scene ([Unity, le impostazioni del bake](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/lightmaps-reference)).
- **Filtering**: il filtro che toglie il rumore dopo il bake, il **denoiser**. Su **Auto** Unity usa OptiX se il PC lo regge, se no OpenImageDenoise. OptiX funziona solo con una scheda NVIDIA, su Windows (stessa pagina). Unity consiglia di usare meno campioni e lasciare al filtro il rumore che resta ([Unity, ottimizzare il bake](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/gpuprogressive-lightmapper)).

**Generate Lighting.** In fondo alla scheda **Scene** c'è il pulsante **Generate Lighting**: fa partire il bake. La barra di avanzamento compare in basso a destra, nella barra di stato dell'editor. Alla fine Unity mette nella cartella del progetto i dati della luce, le lightmap e le reflection probe ([Unity, cuocere la luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/bake)). Lo stesso pulsante cuoce anche le light probe e le reflection probe della scena. Nel suo menu a tendina ci sono **Bake Reflection Probes**, che cuoce solo i riflessi, e **Clear Baked Data**, che cancella quello che hai cotto ([Unity, la finestra Lighting](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-reference/lighting-window)).

**Niente più Auto Generate.** In Unity 6 la casella **Auto Generate**, che rifaceva il bake da sola a ogni cambio, non c'è più. Il bake lo lanci sempre tu, con **Generate Lighting**. Se vuoi un'anteprima mentre lavori, nell'overlay **Lighting Visualization** della Scene view metti **Lighting Data** su **Preview**: Unity usa delle lightmap provvisorie e le rifà quando cambi la scena ([Unity, passare a Unity 6](https://docs.unity.com/en-us/engine/6000.3/manual/upgrade-guides/upgrade-guide-unity6) e [Unity, le viste della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-reference/givis)). Nella finestra **Lighting** c'è anche **Bake On Scene Load**. Su **Never**, il valore di un progetto nuovo di Unity 6, Unity non cuoce niente da solo quando apri una scena ([Unity, la finestra Lighting](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-reference/lighting-window)). Lascialo così.

**Il cielo.** Da Unity 6 anche la luce del cielo si aggiorna solo col bake. Unity: il lightmapper non cuoce più da solo la luce dell'ambiente e il riflesso del cielo, e se cambi le impostazioni dell'ambiente devi rifare **Generate Lighting** per vedere il cambio ([Unity, passare a Unity 6](https://docs.unity.com/en-us/engine/6000.3/manual/upgrade-guides/upgrade-guide-unity6)). Per questo le stanze del faro sono già state cotte una volta, senza lightmap, solo per avere la luce del tramonto. Se cambi il cielo, ricuoci.

**Dopo il bake, regola l'intensità.** Cotta, la luce delle lampade rimbalza sui muri e sul soffitto, e si somma. Ma cambia anche la luce del cielo: in tempo reale arrivava ovunque, anche dentro una stanza chiusa; cotta, entra solo dalle finestre. Nel faro le stanze cotte vengono più scure e più calde di prima: la luce che rimbalza sul legno prende il colore del legno. Se la stanza è troppo buia, seleziona le luci, alza **Intensity** nel componente **Light** e rifai il bake; se ha perso il buio negli angoli, abbassala. Il cliente vuole la sera: la luce calda delle lampade, il buio negli angoli, il tramonto dalla finestra. Regolare le luci fa parte della cura.

**L'emissione.** Un materiale può fare luce da solo: si chiama **emissione**. Meta: i materiali emissivi emettono la loro luce, e si usano per le cose che brillano, come le insegne al neon ([Meta, Lighting and effects](https://developers.meta.com/horizon/design/lighting-and-effects/)). Nei materiali di URP, sotto **Emission**, il menu **Global Illumination** decide cosa fa questa luce sugli altri oggetti ([Unity, lo shader Lit](https://docs.unity.com/en-us/engine/6000.3/manual/materials-and-shaders/built-in/shaders-in-universalrp/reference/lit-shader)):
- **Realtime**: Unity la calcola mentre il gioco gira, ma solo con la GI in tempo reale, che non usiamo;
- **Baked**: Unity la cuoce nelle lightmap;
- **None**: gli altri oggetti non la ricevono.

Unity avverte anche che l'emissione pesa sul batching (stessa pagina). La luce emissiva cotta illumina solo gli oggetti statici; le cose che si muovono la prendono dalle light probe ([Unity, i materiali emissivi](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-birp/lighting-emissive-materials)). E negli interni con superfici emissive ricordati di **Indirect Samples**: forse va alzato.

Nel soggiorno la brace della stufa, `Brace`, e il quadrante della radio, `Quadrante`, brillano, ma non illuminano niente: il loro materiale ha **Global Illumination** su **Realtime**, e la GI in tempo reale nel progetto è spenta.

**La vista Baked Lightmap.** Per vedere le lightmap sulla scena ([Unity, le viste della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-reference/givis) e [Unity, l'overlay Draw Modes](https://docs.unity.com/en-us/engine/6000.3/manual/working-with-scenes/scenes-manage-gameobjects/overlays/reference/draw-modes)):
1. Nella Scene view, nell'overlay **Draw Modes**, una delle barre che galleggiano sopra la vista, clicca l'icona con l'insetto, dopo le quattro sfere: passandoci sopra col mouse dice **Debug Draw Mode**. Si apre l'elenco delle viste ([Unity, l'overlay Debug Mode](https://docs.unity.com/en-us/engine/6000.3/manual/working-with-scenes/scenes-manage-gameobjects/overlays/reference/debug-mode)).
2. Nel gruppo **Baked Global Illumination** scegli **Baked Lightmap**. Vedi solo la luce cotta, senza i colori dei materiali.
3. Per tornare alla vista di sempre, nello stesso overlay clicca la quarta sfera, **Shaded Draw Mode**.

Unity avverte che con URP non tutte le viste compaiono: per il resto c'è il **Rendering Debugger** (stessa pagina).

**Il lightmapper sulla scheda video.** Per il **Progressive GPU** servono ([Unity, CPU o GPU](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/configure/progressive-lightmapper)):
- una scheda video che supporti OpenCL 1.2, il sistema che Unity usa per farle fare i calcoli;
- almeno 2 GB di memoria video tutti per lei;
- un processore con le istruzioni SSE4.1.

Se la scena chiede più memoria video di quella che c'è, il bake può allungarsi molto (stessa pagina). Chiudi gli altri programmi che usano la scheda video ([Unity, ottimizzare il bake](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/gpuprogressive-lightmapper)). Se la tua scheda video non basta, metti **Lightmapper** su **Progressive CPU**: il processore ci mette di più, ma arriva in fondo.

**Il soggiorno · il prima.**
1. Premi **Play** e scegli **Display 2**. Leggi **Batches** e guarda **Shadow casters**.
2. Esci dal Play. Apri il **Light Explorer** e conta le luci **Realtime**: all'inizio sono tutte.
3. Scrivi i due numeri nel file, alla riga del soggiorno.
4. Fai la foto del prima col pulsante del tutorial.

**Il soggiorno · la cura.** Tutto fuori dal Play. Prima gli statici:
1. Nella Hierarchy seleziona `Stanza`.
2. In alto a destra nell'Inspector metti la spunta alla casella **Static**. Unity chiede se vale anche per i figli: rispondi **Yes, change children**. La spunta accende tutti i flag, **Contribute GI** e **Reflection Probe Static** compresi. Per i muri e i mobili fermi va bene così.
3. Fai lo stesso con `Laboratorio`, la radice dello scaffale.
4. `Oggetti` e `Vetri` non li tocchi.

Poi le luci:
1. Nella Hierarchy apri `Luci`. Clicca la prima luce, poi **Shift**+clic sull'ultima: sono tutte selezionate.
2. Nell'Inspector, nel componente **Light**, metti **Mode** su **Baked**. Le ombre lasciale accese: col bake finiscono nella lightmap, morbide, e mentre il gioco gira non costano niente.

Poi la brace e il quadrante:
1. Nella casella di ricerca della Hierarchy scrivi `Brace` e seleziona l'oggetto. In fondo all'Inspector apri il suo materiale.
2. Sotto **Emission**, nel menu **Global Illumination**, scegli **Baked**.
3. Fai lo stesso con `Quadrante`, poi svuota la ricerca con la crocetta.

Poi le impostazioni:
1. Apri **Window ▸ Rendering ▸ Lighting**, scheda **Scene**. Nel campo **Lighting Settings Asset** c'è l'asset `Soggiorno`.
2. **Baked Global Illumination** acceso, **Realtime Global Illumination** spento.
3. Sotto **Lightmapping Settings**: **Lightmapper** su **Progressive GPU**, **Lightmap Resolution** a `10`, **Lightmap Compression** su **Normal Quality**, **Directional Mode** su **Non-Directional**. Il resto lascialo com'è.

Infine il bake:
1. Salva la scena con **Ctrl+S**.
2. In fondo alla finestra **Lighting** premi **Generate Lighting**. Guarda la barra in basso a destra e aspetta che finisca. Nel file scrivi quanto è durato.
3. Guarda la Game view sul **Display 2**. Le lampade fanno le ombre morbide, la luce rimbalza sui muri, la brace illumina il pavimento davanti alla stufa.
4. Se la stanza è troppo buia, seleziona di nuovo le luci in `Luci`, alza **Intensity** e rifai **Generate Lighting**; se ha perso il buio negli angoli, abbassala.
5. Salva con **Ctrl+S**.

Poi rileggi i numeri in Play. **Batches** è sceso di molto, **Shadow casters** è a zero, e nel Light Explorer non c'è più nessuna luce **Realtime**. Ma guarda bene: le cose che si muovono sono scure, lo specchio riflette il cielo, e sullo scaffale il ripiano più basso ha un'ombra che non è sua. Sono le prossime tre sezioni.

## 5. Le cose che si muovono
**Il problema.** Dopo il bake la torcia, il binocolo e le altre cose che si muovono sono scure e piatte, come staccate dalla stanza. Non sono statiche, quindi non hanno la lightmap. E Unity lo scrive: le luci cotte non danno né luce né ombra agli oggetti che si muovono ([Unity, i modi della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/light-modes/introduction)). Senza nient'altro, prendono solo la luce del cielo.

**Le light probe.** Una **light probe** è una sonda: un punto nell'aria che, durante il bake, registra la luce che passa di lì, da tutte le direzioni. Unity spiega la differenza così: le lightmap tengono la luce che arriva sulle superfici, le light probe quella che passa nello spazio vuoto ([Unity, le light probe](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/light-probes)). Il loro primo uso è dare una buona luce, rimbalzi compresi, alle cose che si muovono. Il secondo è illuminare gli oggetti fermi che usano i livelli di LOD della lezione 6.

**Come le usa un oggetto.** Unity prende le probe più vicine all'oggetto e ne fa un tetraedro, una piramide con quattro punte. La luce dell'oggetto è un miscuglio della luce di quelle quattro probe ([Unity, le probe e le cose che si muovono](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/moving-objects)). Unity misura dal centro della scatola che contiene la mesh. Nel **Mesh Renderer** puoi scegliere un altro punto, col campo **Anchor Override** ([Unity, le light probe nel Mesh Renderer](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/mesh-renderer)). Così, quando porti il binocolo vicino alla stufa, il binocolo prende la luce della brace.

**Le regole.**
- [Meta, Android Development](https://developers.meta.com/horizon/documentation/unity/unity-mobile-performance-intro/): per i personaggi e le cose che si muovono, preferisci le light probe alla luce in tempo reale.
- [Meta, Lighting and effects](https://developers.meta.com/horizon/design/lighting-and-effects/): le light probe catturano la luce in punti precisi della scena.
- [Meta, la luce di Batman: Arkham Shadow](https://developers.meta.com/horizon/blog/gdc-2025-design-strategies-considerations-tips-mr-vr-horizon-os/): per gli oggetti che si muovono, Camouflaj ha usato le light probe di Unity.
- [Unity, mettere le light probe](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/class-light-probe-group): metti meno probe che puoi, ma abbastanza da registrare i cambi di luce da un posto all'altro. Più fitte dove la luce cambia tanto o ha forti contrasti, più rade dove resta uguale.
- [Unity, stessa pagina](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/class-light-probe-group): una griglia regolare è semplice e funziona, ma di solito spreca memoria e ha tante probe inutili.
- [Unity, stessa pagina](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/class-light-probe-group): anche se il gioco si svolge su un piano, le probe devono formare un volume. Almeno due strati, uno sopra l'altro.
- [Unity, i problemi delle light probe](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/troubleshooting): vicino a un muro, metti due probe da ogni parte, perché la luce non lo attraversi. E se un angolo buio diventa chiaro, perché la luce di una lampada scavalca il buio, mancano probe nel buio: aggiungile lì.

**Il Light Probe Group.** Le probe vivono in un componente, il **Light Probe Group** ([Unity, mettere le light probe](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/class-light-probe-group)). Si parte da otto probe disposte a cubo, e poi si modellano: si spostano, si duplicano, si cancellano ([Unity, il Light Probe Group](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/reference)).

**Il soggiorno · le probe.**
1. Dal menu in alto apri **GameObject ▸ Light ▸ Light Probe Group**. Nella Scene view compaiono otto palline, disposte a cubo.
2. Nella Scene view, a sinistra, c'è la colonna degli strumenti, che Unity chiama **Tools**. Col gruppo selezionato, sotto gli strumenti per muovere e ruotare compare la sua icona gialla, e sotto ancora il pulsante **Edit Light Probe Group**. Premilo.
3. In basso a destra nella Scene view compare il riquadro **Edit Light Probes**, coi pulsanti **Add Probe**, **Select All**, **Delete Selected** e **Duplicate Selected**. Ora puoi cliccare le probe una per una. L'Inspector del Light Probe Group te lo ricorda: le probe si spostano con quello strumento.
4. Premi **Select All** e trascina tutto il cubo sopra il tavolo, con le frecce.
5. Per farne altre, seleziona qualche probe e premi **Duplicate Selected**: le copie nascono nello stesso punto. Trascinale dove servono.
6. Fai due strati: uno all'altezza del tavolo e della stufa, uno più in alto, all'altezza delle mani.
7. Mettine di più attorno alla stufa, dove la brace cambia la luce, e vicino alla finestra; meno in mezzo alla stanza, dove la luce resta uguale. Mettile nell'aria, dove passano le cose che si muovono: ognuna deve avere delle probe tutt'intorno.
8. Torna alla finestra **Lighting** e premi **Generate Lighting**: anche le probe si cuociono.
9. Premi **Play** e guarda sul **Display 1**, col tuo rig. Guarda il binocolo col mirino e tieni premuto il tasto sinistro del mouse: lo prendi. Portalo vicino alla stufa: prende il colore caldo della brace. Allontanalo: torna in ombra. Quando lasci il tasto, cade. Esci dal Play e salva con **Ctrl+S**.

**La scala: le probe a mano.** Nella scala una griglia non serve: la salita gira attorno alla colonna. Segui i gradini. Un Light Probe Group, poi un gruppetto di probe duplicato e spostato su, un pezzo di salita alla volta, sempre su due strati. Più fitte vicino alle applique e attorno alle cose lasciate sui gradini.

**Due cose che non usiamo.**
- I **Light Probe Proxy Volume** esistono solo nella pipeline vecchia di Unity: in URP non ci sono ([Unity, il confronto fra le pipeline](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/choose-a-render-pipeline/feature-comparison) e [Unity, i Light Probe Proxy Volume](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-birp/light-probe-proxy-volume/class-light-probe-proxy-volume)).
- Le **Adaptive Probe Volumes** sono un sistema nuovo. Riempiono lo spazio di probe da sole, a mattoni da 64 probe, e ogni pixel di un oggetto prende la luce dalle otto probe più vicine, invece che tutto l'oggetto da quattro. Però le probe non si possono spostare a mano, e un Light Probe Group non si converte ([Unity, le Adaptive Probe Volumes](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/probevolumes/concept)). Si accendono nell'asset di URP, sotto **Lighting ▸ Light Probe Lighting**, con **Light Probe System** su **Adaptive Probe Volumes** ([Unity, usarle](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/probevolumes/use)). Né Unity né Meta dicono come vanno sul Quest: le cito, ma non le usiamo. Nel nostro progetto resta **Light Probe Groups (Legacy)**, il sistema dei Light Probe Group.

## 6. I riflessi
**Il problema.** Guarda lo specchio e il bollitore: riflettono il cielo, anche se stanno in una stanza chiusa. Unity spiega perché: quando un punto di un oggetto sta fuori da ogni reflection probe, Unity usa il riflesso del cielo ([Unity, le reflection probe in URP](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/reflection-probes/introduction)).

**La reflection probe.** Una **reflection probe** è una macchina fotografica a 360 gradi. Dal suo centro fotografa la stanza nelle sei direzioni, e ne fa un **cubemap**, un'immagine a forma di cubo. Le superfici lucide lì intorno riflettono quell'immagine invece del cielo. Unity: ognuna delle sei facce si disegna per conto suo, con una camera nel centro della probe ([Unity, ottimizzare i riflessi](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/reflections/ref-probe-performance)).

**Cotta o in tempo reale.** Nella reflection probe il campo **Type** ha tre scelte: **Baked**, **Custom** e **Realtime** ([Unity, la reflection probe](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/reflections/class-reflection-probe)).
- **Baked**: la foto si fa una volta, nel bake. Ci entrano solo gli oggetti con **Reflection Probe Static**: le cose che si muovono nel riflesso non ci sono.
- **Realtime**: la foto si rifà mentre il gioco gira. Unity avverte: le probe in tempo reale non sono compresse in memoria, e leggerle costa di più che leggere quelle cotte. Il modo più caro per il processore è **Every Frame**, una foto nuova a ogni frame ([Unity, ottimizzare i riflessi](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/reflections/ref-probe-performance)).

Le probe cotte invece sono compresse: la compressione la trovi nella finestra **Lighting**, scheda **Environment**, sotto **Environment Reflections**, alla voce **Compression**, che parte da **Auto** (stessa pagina). Nel faro le reflection probe sono tutte **Baked**.

**La scatola.** Ogni probe agisce dentro una scatola, grande quanto dice **Box Size**: per una stanza, grande come la stanza. Con **Box Projection** acceso il riflesso cambia secondo dove sta l'oggetto nella scatola, e lo specchio mostra la stanza dal punto giusto. Unity: in URP la **Box Projection** va accesa anche nell'asset di URP ([Unity, la reflection probe](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/reflections/class-reflection-probe)). Nel nostro progetto è già accesa, nei due profili.

**La fascia che sfuma.** Vicino alle pareti della scatola il riflesso della probe sfuma verso il cielo, o verso un'altra probe. Quanto è larga la fascia lo dice **Blend Distance**, e Unity parte da 1 metro. L'ho provato nel soggiorno: con la scatola grande come la stanza e **Blend Distance** a 1, lo specchio appeso al muro sta nella fascia, e dopo il bake riflette ancora quasi solo il cielo. Con una probe sola in una stanza chiusa non c'è niente con cui sfumare: metti **Blend Distance** a 0.1.

**Dove metterla.** Unity: mettila vicino agli oggetti grandi che si vedono nei riflessi; il centro e gli angoli delle pareti sono buoni posti ([Unity, usare le reflection probe](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/reflections/using-reflection-probes)). Meta dà un motivo per usarne poche: per disegnare tanti oggetti in una draw call sola, devono usare tutti la stessa reflection probe ([Meta, RenderDoc](https://developers.meta.com/horizon/documentation/unity/po-renderdoc-optimizations-1/)). Una per stanza basta.

**Quante ne mescola un oggetto.** Unity: un oggetto che sta dentro più scatole prende i riflessi al massimo da due probe ([Unity, le reflection probe in URP](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/reflection-probes/introduction)). Il passaggio morbido fra una probe e l'altra è il **Probe Blending**, nell'asset di URP, nel riquadro **Lighting**. Qui le regole ufficiali sono due, e non dicono proprio la stessa cosa:
- [Unity, configurare URP per le prestazioni](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/introduction/configure-for-better-performance): sulle piattaforme mobili meno potenti, se usi le reflection probe, spegni **Probe Blending** e **Box Projection**.
- [Meta, Forward+ Rendering](https://developers.meta.com/horizon/documentation/unity/unity-forward-plus-rendering/): col Forward+, spegni **Probe Blending** e **Probe Atlas Blending** nell'asset di URP. Il **Probe Atlas Blending** mette i riflessi di tutte le probe in un atlas, una texture sola, e Meta scrive che leggerlo costa troppo, e porta facilmente l'app fuori dal budget del frame. Per il PC, aggiunge Meta, forse li puoi tenere, se le prove dicono che va bene. Il **Probe Atlas Blending** vale solo col Forward+ ([Unity, l'asset di URP](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/urp-reference/universalrp-asset)): il nostro visore usa il Forward.

Nel faro teniamo la **Box Projection**: senza, lo specchio e gli ottoni non mostrerebbero la stanza. È una scelta, e il suo prezzo si misura sul visore.

**Solo i materiali Lit riflettono.** In URP la reflection probe la leggono i materiali **Lit**. I **Simple Lit** prendono la luce cotta e quella delle light probe, ma non i riflessi: lo dice il codice dello shader, che per i Simple Lit non legge l'ambiente riflesso. Per questo nel faro lo specchio, gli ottoni, il bollitore e la bussola sono Lit, e il resto è Simple Lit. Nel tuo progetto: Lit solo dove il riflesso si vede.

**Le luci cotte non fanno brillare.** Le luci **Baked** non fanno il riflesso speculare: sugli oggetti lucidi manca il punto di luce della lampada ([Unity, i modi della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/light-modes/introduction)). La reflection probe lo restituisce: fotografa la stanza illuminata, e gli oggetti lucidi la riflettono. Meta chiede anche di cuocere nelle texture tutti i dettagli che puoi, riflessi compresi ([Meta, Android Development](https://developers.meta.com/horizon/documentation/unity/unity-mobile-performance-intro/)).

**Il soggiorno · la reflection probe.**
1. Dal menu in alto apri **GameObject ▸ Light ▸ Reflection Probe** ([Unity, usare le reflection probe](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/reflections/using-reflection-probes)).
2. Nell'Inspector, **Type** è **Baked**: lascialo.
3. Mettila al centro della stanza, all'altezza degli occhi: nel **Transform**, **Position** a `0`, `1.5`, `0`.
4. **Box Size** a `6.4`, `3.1`, `5.4`: la scatola tocca i muri, il pavimento e il soffitto. Unity parte da 10 metri per lato.
5. Spunta **Box Projection**.
6. **Blend Distance** a `0.1`, come spiegato qui sopra.
7. Nella finestra **Lighting** premi **Generate Lighting**.
8. Guarda lo specchio e il bollitore sul **Display 2**: ora riflettono la stanza.

**Il soggiorno · il dopo.** Premi **Play**, scegli **Display 2** e rileggi **Batches**; nel Light Explorer conta le luci in tempo reale. Scrivili nel file ed esci dal Play. Fai la foto del dopo col pulsante del tutorial e salva la scena con **Ctrl+S**. Poi il curatore, con il compagno di banco, come nella sezione 9.

## 7. Il laboratorio: le UV della luce in Blender
Sul **Display 3**, `Camera Scaffale` inquadra lo scaffale del soggiorno. Dopo il bake il ripiano più basso ha un'ombra scura, larga come la fila di libri del ripiano in alto, e sopra non c'è niente che la faccia: è proprio l'ombra di quei libri. La cura si fa dove nasce il problema, nel programma 3D.

**Perché.** Lo scaffale ha due mappe UV. La prima, `UVMap`, serve alla texture del legno. I quattro ripiani sono uguali, e chi l'ha modellato li ha impilati: stanno tutti sulla stessa **isola**, cioè sullo stesso pezzo di superficie steso sul piano, e usano lo stesso pezzo di legno. Per la texture è un buon trucco: risparmia spazio. La seconda mappa, `Lightmap`, è quella della luce, ed è una copia della prima: anche lì i ripiani sono impilati. Ma la luce non è uguale su ogni ripiano: quello in alto prende la lampada, quello in basso sta in ombra. Con le isole una sopra l'altra, i quattro ripiani si dividono gli stessi texel della lightmap, e ognuno prende anche la luce degli altri.

**Le regole di Unity.** Le UV della luce fatte a mano devono ([Unity, generare le UV della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/lighting-gi-uvs/generating-lightmapping-uvs)):
- stare tutte dentro il quadrato da 0 a 1;
- avere un margine abbastanza largo fra un'isola e l'altra;
- non avere facce sovrapposte;
- deformare poco gli angoli: gli angoli nelle UV simili a quelli del modello;
- deformare poco le grandezze: ogni triangolo, nelle UV, grande in proporzione come nel modello. A meno che tu non voglia dare più risoluzione a una parte.

Il lightmapper di Unity lo ripete: vuole isole che non si sovrappongono, poco deformate, con un margine sufficiente fra l'una e l'altra ([Unity, i lightmapper](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/lightmappers)). Nel manuale di Unity le isole si chiamano chart.

**La vista UV Overlap.** Unity ha una vista che colora di rosso i texel della lightmap troppo vicini a un texel di un'altra isola: sono quelli che danno macchie, scalini e altri difetti ([Unity, le viste della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-reference/givis)). Si sceglie nella Scene view, nel gruppo **Baked Global Illumination** delle modalità di disegno. Ma le isole impilate esatte, come i ripiani dello scaffale, non le vede. L'ho provato nel soggiorno cotto: lo scaffale resta grigio, e a 10 texel per metro si colorano di rosso i libri, la pentola, le sedie, le cose piccole. Le sovrapposizioni vere le trovi in Blender, con **Select Overlap**.

Per la luce che sbava fra le isole Unity consiglia proprio questo: se le UV le fai a mano, allarga i margini nel programma 3D, e usa la vista **UV Overlap** per trovare i texel che danno problemi ([Unity, i difetti delle lightmap](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/troubleshooting/troubleshooting-lightmapping-artifacts)).

**Il prima.** Fai la foto del prima dello scaffale col pulsante del tutorial.

**In Blender · importa.**
1. Apri Blender. Col mouse sulla vista 3D premi **A**, poi **X**, e scegli **Delete**: via il cubo, la camera e la luce. Così nella scena resta solo lo scaffale.
2. Dal menu **File ▸ Import ▸ FBX (.fbx)** apri `scaffale.fbx`. Sta nella cartella della tua copia, in `sandbox/Assets/Demo/Models/Faro`.
3. Clicca lo scaffale per selezionarlo: è un oggetto solo.

**In Blender · la mappa giusta.**
1. A destra, nelle **Properties**, apri la scheda **Object Data**, quella col triangolo verde, e il riquadro **UV Maps**. Ci sono due mappe: `UVMap` e `Lightmap`.
2. Clicca `Lightmap`: si evidenzia. È quella che vedi e cambi nell'editor delle UV ([Blender, le UV map](https://docs.blender.org/manual/en/4.5/modeling/meshes/uv/uv_texture_spaces.html)).
3. Guarda l'icona della fotocamera: resta su `UVMap`. La fotocamera segna la mappa che i materiali usano per disegnare la texture. Non spostarla.
4. In alto passa alla scheda **UV Editing**: Blender entra da solo in Edit Mode.
5. Col mouse sulla vista 3D, a destra, premi **A**: tutto selezionato. Nell'editor delle UV, a sinistra, vedi la mappa `Lightmap`: i quattro ripiani sono uno sopra l'altro.

**In Blender · Lightmap Pack.** **Lightmap Pack** dà a ogni faccia il suo posto nel quadrato, e lascia uno spazio vuoto fra l'una e l'altra ([Blender, gli operatori UV](https://docs.blender.org/manual/en/4.5/modeling/meshes/editing/uv.html)).
1. Nel menu in alto della vista 3D apri **UV ▸ Lightmap Pack**. Lo stesso menu si apre col tasto **U**, col mouse sulla vista 3D. Nelle versioni più nuove di Blender la voce sta in **UV ▸ Unwrap ▸ Lightmap Pack**.
2. Si apre una finestrella con le opzioni:
   - **Selection**: lascia **Selected Faces**. Hai selezionato tutto, quindi vale per tutte le facce;
   - **Share Texture Space**: lascialo acceso. Serve quando impacchetti più oggetti insieme, perché le loro UV non si sovrappongano;
   - **New UV Map**: spento. Acceso, Blender farebbe una mappa nuova invece di riscrivere quella selezionata, `Lightmap`;
   - **Pack Quality**: lascia **12**. Più è alto, meno spazio si spreca, e più ci mette;
   - **Margin**: lo spazio vuoto fra una faccia e l'altra. Blender parte da 0.1: scrivi **0.5**, che fra una faccia e l'altra lascia circa cinque pixel di una lightmap da 512. Più la lightmap ha pochi texel per metro, più margine serve.
3. Premi **OK**. Nell'editor delle UV ogni faccia ha ora il suo posto, con lo spazio intorno. Questo comando non ha il riquadro per cambiare i valori dopo: se vuoi un altro margine, **Ctrl+Z** e rifallo.

**In Blender · il controllo.**
1. Col mouse sull'editor delle UV premi **Alt+A**: niente selezionato.
2. Nel menu dell'editor delle UV scegli **Select ▸ Select Overlap**, l'ultima voce. Blender seleziona tutte le facce delle UV che si sovrappongono ([Blender, selezionare le UV](https://docs.blender.org/manual/en/4.5/editors/uv/selecting.html)). Se non si accende niente, va bene. Se si accende qualcosa, rifai **Lightmap Pack**.
3. Col mouse sulla vista 3D premi **Tab**: torni in Object Mode.

**In Blender · esporta.** Come sempre ([Blender, FBX](https://docs.blender.org/manual/en/4.5/addons/import_export/scene_fbx.html)):
1. Con lo scaffale selezionato, **File ▸ Export ▸ FBX (.fbx)**.
2. A destra, sotto **Include**, spunta **Selected Objects**: esporti solo lo scaffale. Sotto **Transform**, in **Apply Scalings**, scegli **FBX Units Scale**: se no lo scaffale arriva in Unity cento volte più piccolo.
3. Salva sopra `scaffale.fbx`, nella cartella `sandbox/Assets/Demo/Models/Faro`, con lo stesso nome.

Se qualcosa è andato storto in Blender, ricomincia in un file nuovo: **File ▸ New ▸ General**, e importa di nuovo il modello. Se l'hai già sovrascritto, nella cartella della tua copia lancia `git checkout -- sandbox/Assets/Demo/Models/Faro/scaffale.fbx` e il modello torna com'era.

**In Unity · il dopo.**
1. Torna in Unity: lo scaffale si aggiorna da solo.
2. Nella finestra **Project** apri `Assets/Demo/Models/Faro` e seleziona `scaffale`. Nell'Inspector, scheda **Model**: **Generate Lightmap UVs** deve restare spento. Se lo accendi, Unity scrive le sue UV nel secondo canale, sopra le tue.
3. Nella Hierarchy seleziona `Laboratorio` e controlla che lo scaffale sia ancora statico. Se la casella **Static** è vuota, rimetti la spunta, con **Yes, change children**.
4. Nella finestra **Lighting** premi **Generate Lighting**.
5. Guarda il **Display 3**: l'ombra sul ripiano più basso è sparita, e ogni ripiano ha la sua luce. Se resta, torna in Blender, controlla con **Select Overlap** e rifai **Lightmap Pack** con un **Margin** più alto.
6. Fai la foto del dopo e salva la scena con **Ctrl+S**.

Nel file delle misure scrivi cosa si vedeva sullo scaffale prima e cosa dopo, e il margine che hai dato.

Il modello riesportato sta in `Assets/Demo/Models/Faro`: al prossimo `git aggiorna` quella cartella torna come prima. È un esercizio: le tue foto e il file delle misure restano. Le UV della luce per il tuo progetto falle sui tuoi modelli, nella cartella del progetto.

## 8. Trucchi del mestiere
Nel programma 3D:
- **Una seconda UV per ogni oggetto fermo.** La prima per la texture, la seconda per la luce. Se non la fai tu, la fa Unity con **Generate Lightmap UVs**: comodo, ma decide lui dove tagliare.
- **Mai isole impilate o specchiate nella mappa della luce.** Nella texture due parti uguali possono stare sulla stessa isola, o su un'isola specchiata, e risparmi spazio. Nella lightmap no: ogni faccia ha la sua luce, e le serve il suo posto. Unity vuole zero facce sovrapposte ([Unity, generare le UV della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/lighting-gi-uvs/generating-lightmapping-uvs)).
- **Il margine pensato per la lightmap.** Con pochi texel per metro un texel è grande, e due isole vicine finiscono nello stesso texel. Unity misura il margine in pixel: nelle UV che genera, il valore di partenza è 4 pixel, come se la mesh occupasse una lightmap intera di 1024 × 1024. Con **Margin Method** su **Calculate**, Unity lo calcola da solo, dalla risoluzione della lightmap e dalla grandezza dell'oggetto ([Unity, le impostazioni delle UV della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/lighting-gi-uvs/reference)).
- **Le cuciture negli angoli.** Dove un'isola finisce, la luce può avere uno stacco. Metti le cuciture negli angoli e sotto i mobili, dove non si vedono. Per gli stacchi che restano, nel **Mesh Renderer**, sotto **Lightmapping**, c'è **Stitch Seams**, che cuce le cuciture della lightmap ([Unity, il Mesh Renderer](https://docs.unity.com/en-us/engine/6000.3/manual/assets-and-media/asset-types/mesh/components-reference/class-mesh-renderer)).
- **I muri con lo spessore e gli angoli chiusi.** Un muro fatto di un piano solo lascia filtrare la luce, e così un angolo dove due muri non si toccano. Dai spessore ai muri e chiudi gli angoli. Per le superfici sottili con una faccia sola, Unity propone **Double Sided Global Illumination** ([Unity, i difetti delle lightmap](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/troubleshooting/troubleshooting-lightmapping-artifacts)).
- **L'ombra dipinta.** Le cose che si muovono non hanno la lightmap. Il buio nei loro incavi, l'**occlusione ambientale**, dipingilo nella texture. Meta: cuoci nelle texture più dettagli che puoi, come i riflessi e l'occlusione ambientale ([Meta, Android Development](https://developers.meta.com/horizon/documentation/unity/unity-mobile-performance-intro/)).
- **I LOD con le UV della luce del LOD0.** Se un oggetto ha i livelli di dettaglio della lezione 6, Meta chiede che tutti usino la stessa lightmap, e che le UV della luce di ogni livello coincidano con quelle del modello pieno, il LOD0 ([Meta, Open World Games and Asset Streaming](https://developers.meta.com/horizon/documentation/unity/po-assetstreaming/)).
- **Più spazio a quello che si vede da vicino.** Unity vuole le grandezze poco deformate, a meno che tu non voglia dare più risoluzione a una parte ([Unity, generare le UV della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/lighting-gi-uvs/generating-lightmapping-uvs)). Il piano di un tavolo che il giocatore guarda da vicino può avere un'isola più grande del suo retro.
- **Smart UV Project.** Per un modello più complicato, **Smart UV Project** divide la superficie in isole secondo l'angolo fra le facce, **Angle Limit**, e lascia spazio fra le isole, **Island Margin** ([Blender, gli operatori UV](https://docs.blender.org/manual/en/4.5/modeling/meshes/editing/uv.html)). Le regole di Unity restano le stesse.

In Unity:
- **Cuoci presto e spesso, a bassa risoluzione.** Prova le luci con un bake veloce, con pochi texel e pochi campioni; alza solo alla fine. Per un bake più veloce Unity consiglia: chiudi gli altri programmi che usano la scheda video; usa meno campioni, e lascia il rumore al filtro; abbassa la risoluzione; usa le light probe per gli oggetti piccoli ([Unity, ottimizzare il bake](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/gpuprogressive-lightmapper)).
- **Guarda i texel.** Nell'overlay **Lighting Visualization** della Scene view, **Show Lightmap Resolution** disegna una scacchiera sopra gli oggetti: ogni quadretto è un pixel della lightmap ([Unity, le viste della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-reference/givis)).
- **Chi è nel bake.** La vista **Contributors / Receivers** colora gli oggetti: arancione chi non entra nella GI, verde chi la prende dalle lightmap, blu chi la prende dalle light probe (stessa pagina).
- **Scale In Lightmap.** Nel **Mesh Renderer**, sotto **Lightmapping**: quanto spazio ha l'oggetto nella lightmap. Sopra 1 ha più texel, sotto 1 meno. A 0 non ha lightmap, ma fa ancora la sua parte nella luce degli altri ([Unity, il Mesh Renderer](https://docs.unity.com/en-us/engine/6000.3/manual/assets-and-media/asset-types/mesh/components-reference/class-mesh-renderer)). Sotto 1 è per le cose piccole o lontane; sopra 1 per quelle che si guardano da vicino. Nel faro lo scaffale ha 6: lo guardi da vicino, sul **Display 3**, e a 10 texel per metro ogni ripiano avrebbe pochi pixel.
- **Gli oggetti minuti dalle light probe.** Sassi, rottami, attrezzi piccoli occupano posto nelle lightmap e cambiano poco l'aspetto della scena. Unity consiglia di spegnere il loro **Contribute Global Illumination** e di illuminarli con le light probe ([Unity, ottimizzare il bake](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/gpuprogressive-lightmapper)). C'è una via di mezzo: lascia acceso **Contribute GI** e metti **Receive Global Illumination** su **Light Probes**. Restano nei calcoli del bake, ma la loro luce la prendono dalle probe ([Unity, il Mesh Renderer](https://docs.unity.com/en-us/engine/6000.3/manual/assets-and-media/asset-types/mesh/components-reference/class-mesh-renderer)). In tutti e due i casi, mettigli delle probe vicino.
- **La memoria delle lightmap.** Le lightmap direzionali occupano circa il doppio di quelle non direzionali; contano anche la compressione e la grandezza massima ([Unity, le impostazioni del bake](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/lightmaps-reference)). Meta, raccontando un suo progetto, ricorda che le lightmap possono occupare tanta memoria, e che per questo vanno tenute compresse; e che abbassare **Lightmap Resolution** riduce il numero delle lightmap ([Meta, Open World Games and Asset Streaming](https://developers.meta.com/horizon/documentation/unity/po-assetstreaming/)). Per il visore c'è anche **Lightmap Encoding**, in **Edit ▸ Project Settings ▸ Player**, sotto **Other Settings**. **High Quality** tiene la luce in HDR, anche quella molto forte; **Normal Quality** usa la codifica RGBM; **Low Quality**, sui dispositivi mobili, taglia la luce cotta più forte di 2. Su Android Unity comprime le lightmap in ASTC, e nella sua tabella le tre codifiche pesano uguale: 3,56 bit per pixel ([Unity, il formato delle lightmap](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmap-data/lightmaps-technical-information)).
- **Lightmap diverse spezzano lo static batching.** Meta: le lightmap possono spezzare i gruppi dello static batching, quindi è importante che le mesh usino la stessa lightmap quando possono ([Meta, Open World Games and Asset Streaming](https://developers.meta.com/horizon/documentation/unity/po-assetstreaming/)). Lo hai visto nella lezione 7: Unity unisce solo oggetti con la stessa lightmap. Meta racconta che per raggrupparli ha usato i **Lightmap Parameters**, col parametro **System Tag**.
- **Una luce che si muove resta viva, e senza ombre.** Il fascio della lanterna gira: cotto, resterebbe fermo nella lightmap. Resta **Realtime**, ma senza ombre: sul visore le luci aggiuntive non le fanno comunque. Lo stesso vale per una lampada che si accende con un interruttore: una luce cotta, mentre il gioco gira, non si può cambiare ([Unity, i modi della luce](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/light-modes/introduction)).
- **Una luce solo per certi oggetti.** I **Rendering Layers** fanno illuminare a una luce solo gli oggetti che scegli. Col Forward, se li usi solo per le luci, Unity scrive che il costo è trascurabile; cresce quando i layer arrivano a 9, 17, 25 ([Unity, i Rendering Layers](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/rendering-layers/introduction)). Nella luce si scelgono in **Rendering Layers**; nell'oggetto in **Rendering Layer Mask**, sotto **Additional Settings** ([Unity, le luci e i Rendering Layers](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/rendering-layers/lights)).
- **I cookie al posto delle ombre.** Un **cookie** è una texture davanti alla luce, come una mascherina: disegna luce e buio, per esempio l'ombra di una grata. Unity: dove gli oggetti sono quasi tutti fermi, una luce Point o Spot col cookie può prendere il posto di una luce con le ombre in tempo reale ([Unity, ottimizzare le ombre in URP](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/shadows-in-urp/shadows-optimization)).
- **L'ombra finta.** Se un'ombra in tempo reale ti serve davvero, falla fare a una versione semplice dell'oggetto, con **Cast Shadows** su **Shadows Only**: non si vede, ma fa l'ombra (stessa pagina). Anche in Batman: Arkham Shadow le ombre dei personaggi sulla scena cotta sono finte, fatte con una tecnica loro ([Meta, la luce di Batman: Arkham Shadow](https://developers.meta.com/horizon/blog/gdc-2025-design-strategies-considerations-tips-mr-vr-horizon-os/)).
- **Baked Lit per gli oggetti fermi.** Sulle piattaforme mobili meno potenti Unity consiglia lo shader **Baked Lit** per gli oggetti fermi e **Simple Lit** per quelli che si muovono ([Unity, configurare URP per le prestazioni](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/introduction/configure-for-better-performance)). **Baked Lit** prende solo la luce cotta, nelle lightmap e nelle light probe, senza luci in tempo reale.
- **Una reflection probe per stanza.** Gli oggetti che usano la stessa probe si possono disegnare insieme ([Meta, RenderDoc](https://developers.meta.com/horizon/documentation/unity/po-renderdoc-optimizations-1/)).
- **I riflessi spenti dove non servono.** Nei materiali **Lit**, togliere la spunta a **Environment Reflections** rende lo shader più veloce, ma toglie i riflessi ([Unity, lo shader Lit](https://docs.unity.com/en-us/engine/6000.3/manual/materials-and-shaders/built-in/shaders-in-universalrp/reference/lit-shader)). Va bene per l'intonaco e il legno opaco.
- **Il cielo.** Meta consiglia di evitare lo skybox di default di Unity, che sul visore autonomo costa. E nella finestra **Lighting** suggerisce di togliere lo skybox e di mettere la luce ambiente su un colore ([Meta, Android Development](https://developers.meta.com/horizon/documentation/unity/unity-mobile-performance-intro/)). Nel faro il cielo del tramonto si vede dalle finestre ed è parte della scena: lo teniamo, e la sua luce è cotta. Dopo ogni cambio del cielo, ricuoci ([Unity, passare a Unity 6](https://docs.unity.com/en-us/engine/6000.3/manual/upgrade-guides/upgrade-guide-unity6)).
- **La stanza buia della lezione 3 non serve più.** Ti ricordi la stanza buia della lezione 3? Per farla buia davvero c'erano voluti un ingresso a gomito e un componente che toglieva agli oggetti la luce del cielo. Con la luce cotta, anche la luce del cielo si calcola nel bake: dove non arriva, la stanza resta buia. E se nella stanza ci sono le light probe, restano al buio anche le cose che si muovono. I muri spessi, quelli sì, servono ancora.
- **La misura vera è sul visore.** Stats sul PC conta le passate del PC. Per i numeri del Quest servono il Profiler collegato al visore e OVR Metrics, come nella lezione 5.

## 9. Il giro del curatore
Per ogni stanza il giro è lo stesso del soggiorno:
1. apri la scena della stanza;
2. in Play, il **Display 2**: **Batches** e le luci in tempo reale nel file delle misure;
3. la foto del prima, col pulsante del tutorial;
4. la cura, fuori dal Play;
5. il bake;
6. di nuovo in Play i numeri nel file, e la foto del dopo.

**Le regole della cura.** Niente si cancella, si spegne o si sposta: lampade, mobili e oggetti restano dove sono. Puoi cambiare il modo delle luci, l'intensità, il colore e le ombre, i flag **Static** e le impostazioni del bake. Puoi aggiungere light probe e reflection probe.

**Il curatore sceglie la foto.** Nelle lezioni 6 e 7 il curatore indovinava qual era la foto curata. Oggi sarebbe troppo facile: la luce cotta di solito è più bella. Quindi il curatore sceglie la foto che metterebbe nel gioco. Gli mostri le due foto a tutto schermo, nell'ordine che decidi tu e senza fargli vedere i nomi dei file. Lui sceglie, e dice perché.

Deve restare l'atmosfera del cliente: la luce calda delle lampade, il buio negli angoli, il tramonto dalla finestra. Se sceglie la foto del prima, ascolta il perché e torna nella stanza:
- la stanza è troppo buia: alza **Intensity** delle luci e ricuoci; se invece è troppo chiara, e la sera è sparita, abbassala. Regolare le luci fa parte della cura;
- la luce è granulosa: alza **Indirect Samples**, o usa il filtro;
- un'ombra che si guarda da vicino è sfocata o a scalini: dai più texel a quell'oggetto, con **Scale In Lightmap**;
- una cosa che si muove sembra spenta: mancano le light probe attorno.

Poi scambiatevi i ruoli. Nel file scrivi quale foto ha scelto il curatore, in ogni stanza, e perché.

**Le tre stanze della verifica.** Ogni stanza ha la sua scena. Quando Unity chiede se salvare la scena di prima, rispondi **Save**. Alla fine di ogni stanza salva con **Ctrl+S**.
- **L'officina**, scena `Officina`, **Display 2**. Il cartello ha due budget: le **Batches** e la memoria delle lightmap. Statiche la `Stanza` e la `Minuteria`, cotte le luci, poi il bake. La memoria la leggi sotto **Generate Lighting**. Per farla stare nel budget: la risoluzione giusta, le lightmap non direzionali e compresse, e gli attrezzi piccoli della `Minuteria` fuori dalle lightmap. Puoi toglierli dal bake, metterli su **Receive Global Illumination: Light Probes**, o dargli poco **Scale In Lightmap**. Se prendono la luce dalle probe, mettigli delle probe vicino. Gli attrezzi non si spostano e non si cancellano.
- **La scala**, scena `Scala`, **Display 2**. Statica la `Stanza`, cotte le luci, il bake. Poi le light probe, a mano, lungo la salita e attorno alle cose lasciate sui gradini. Ricuoci.
- **La lanterna**, scena `Lanterna`, **Display 2**. Statica la `Stanza`, cotte tutte le luci tranne il `Fascio`: lui gira, e resta **Realtime**, senza ombre. Nel componente **Light** metti **Shadow Type** su **No Shadows**. Poi il bake, e una reflection probe cotta per l'ottone: **Position** `0`, `1.6`, `0`, **Box Size** `5.4`, `3.2`, `5.4`, la **Box Projection** e **Blend Distance** a `0.1`. Il centro sta un po' più in alto che nel soggiorno, perché la scatola arrivi sopra le cornici d'ottone in cima ai vetri. Qui il cartello permette una luce in tempo reale: il fascio.

## 10. La consegna
Tutto in `Consegne/L08/`:
- `misure.md`, dal pulsante del tutorial, senza nessun «da scrivere»;
- le foto del faro: `soggiorno-`, `officina-`, `scala-` e `lanterna-prima.jpg` e `-dopo.jpg`;
- le foto del laboratorio: `scaffale-prima.jpg` e `scaffale-dopo.jpg`;
- le foto del tuo progetto: `progetto-prima.jpg` e `progetto-dopo.jpg`, dalla tua `Camera foto`.

Il file delle misure è fatto così:

```
# Le misure della lezione 8

Al posto di ogni segnaposto tra « » scrivi il tuo numero o il tuo testo. Le foto stanno in questa cartella.

## Il faro
Il soggiorno, Batches e luci in tempo reale prima: «da scrivere»
Il soggiorno, Batches e luci in tempo reale dopo: «da scrivere»
Quanto è durato il bake del soggiorno: «da scrivere»
L'officina, Batches prima e dopo, e le lightmap dopo il bake: «da scrivere»
La scala, Batches prima e dopo: «da scrivere»
La lanterna, Batches prima e dopo: «da scrivere»
Il curatore quale foto ha scelto, in ogni stanza, e perché: «da scrivere»

## Il laboratorio
Cosa si vedeva sullo scaffale prima, e cosa dopo: «da scrivere»
Il margine che hai dato alle isole: «da scrivere»

## Il tuo progetto
Dove hai messo la camera, e perché: «da scrivere»
Batches e luci in tempo reale prima: «da scrivere»
Cosa hai cotto, e cosa hai lasciato in tempo reale: «da scrivere»
Batches e luci in tempo reale dopo: «da scrivere»
```

**Il tuo progetto.** Il pulsante del tutorial apre la scena del tuo progetto. Usa la `Camera foto` delle lezioni 6 e 7, sul **Display 2**. Se non c'è, rifalla:
1. Nella Scene view inquadra il punto che conta di più, all'altezza degli occhi.
2. Dal menu in alto scegli **GameObject ▸ Camera** e rinomina la camera `Camera foto`.
3. Con la camera selezionata scegli **GameObject ▸ Align With View**: la camera prende l'inquadratura della Scene view.
4. Nell'Inspector della camera metti **Target Display** su **Display 2**.

Poi premi **Play**, scegli **Display 2**, leggi **Batches** e scrivilo nella parte del progetto, con le luci in tempo reale contate nel Light Explorer. Esci dal Play e fai la foto del prima col pulsante del tutorial. Poi la cura:
- statici i muri e i mobili fermi; non i mattoncini che si muovono: porte, leve, cassetti, oggetti da prendere;
- le luci in **Baked**, tranne quelle che si muovono, come la torcia della lezione 3, e quelle che un interruttore accende: queste restano **Realtime**, e se puoi senza ombre;
- le light probe dove si cammina e dove passano le cose che si muovono;
- una reflection probe per stanza, cotta, con la **Box Projection** e **Blend Distance** a 0.1;
- le impostazioni nella finestra **Lighting**: se la scena non ha un asset suo, prima premi **New Lighting Settings**;
- il bake, e l'intensità delle luci da regolare dopo.

Ci sono anche le scorciatoie del menu **RUFA ▸ Strumenti**:
- **Lighting: preset mobile** mette insieme le impostazioni: GI cotta, il lightmapper sulla scheda video, 10 texel per unità, lightmap fino a 2048 pixel, non direzionali, compressione **Normal Quality**, e il **Lighting Mode** su **Subtractive** per le luci Mixed, se ne hai. Se la scena non ha un asset di impostazioni, lo crea.
- **Light Probe Group a griglia (selezione)** mette dentro l'oggetto selezionato, per esempio il pavimento di una stanza, una griglia di probe su due strati, e salta i punti che cadono dentro i mobili. È un punto di partenza: poi sistemale a mano, con le regole di Unity.
- **Reflection Probe (selezione)** mette una reflection probe cotta, grande come l'oggetto selezionato, con la **Box Projection** e **Blend Distance** a 0.1. Seleziona la radice della stanza, muri compresi.
- **Lighting: bake** salva la scena e fa partire il bake. **Lighting: cancella** toglie la luce cotta, se vuoi rivedere il prima.

Poi i numeri del dopo, e la foto del dopo. Chi vuole fa la build e guarda la scena sul visore. Il budget del tuo progetto è quello di Meta per il tuo tipo di gioco. Se il tuo blockout è già sotto, va bene: la consegna chiede di cuocere, di misurare e di scrivere cosa hai fatto e perché.

A fine lezione, `git salva "Lezione 08"`.

## 11. I budget nella vita vera
- **Meta, i frame al secondo.** Le app interattive devono fare almeno 72 frame al secondo; le app multimediali possono stare a 60 ([Meta, Testing and performance analysis](https://developers.meta.com/horizon/documentation/unity/unity-perf/)). Nella pagina sulla luce Meta scrive almeno 60, e che 90 è l'ideale per la VR ([Meta, Lighting and effects](https://developers.meta.com/horizon/design/lighting-and-effects/)). Il nostro progetto chiede 90.
- **Meta, le draw call e i triangoli.** Sul Quest 3 e sul 3S, da 200 a 300 draw call per frame con tanta simulazione, da 400 a 600 con una simulazione media, da 700 a 1000 con poca. Le escape room stanno nella terza riga. I triangoli, da 1,3 a 1,8 milioni per frame ([Meta, Testing and performance analysis](https://developers.meta.com/horizon/documentation/unity/unity-perf/)).
- **Nessun numero per la luce.** Meta non dà un numero massimo di luci in tempo reale per il Quest. Nemmeno per la distanza delle ombre, per la loro risoluzione, o per la grandezza delle lightmap. Unity nemmeno: dà i limiti tecnici di URP, come le 9 luci per oggetto del Forward, e i valori di partenza delle sue impostazioni, non un budget. L'unico indizio di Meta: il Forward+ comincia a convenire intorno alle 5 luci ([Meta, Forward+ Rendering](https://developers.meta.com/horizon/documentation/unity/unity-forward-plus-rendering/)). Quindi il budget della luce lo decide lo studio, o il cliente: quante luci vive, quanti MB di lightmap, quanti texel per metro. Come il nostro cliente, coi cartelli.
- **Una volta, una luce sola.** In una nota tecnica vecchia, scritta per la pipeline di Unity di allora, Meta chiedeva una sola luce per pixel e ombre solo dure, o spente. Per le ombre dei personaggi, macchie sfocate; tutte le altre ombre, cotte nelle lightmap ([Meta, la nota tecnica sulle impostazioni per la VR mobile](https://developers.meta.com/horizon/blog/tech-note-unity-settings-for-mobile-vr/)). Oggi la pagina vale come storia, ma l'idea è la stessa.
- **Quando due fonti non dicono la stessa cosa.** Per l'MSAA, l'antialiasing che ammorbidisce i bordi seghettati, Meta dice di scegliere 4x ([Meta, Project Configuration Overview](https://developers.meta.com/horizon/documentation/unity/unity-conf-settings/)). La pagina di Unity sui visori autonomi scrive che 2x è un buon equilibrio fra qualità e prestazioni ([Unity, i visori autonomi](https://docs.unity.com/en-us/engine/6000.3/manual/xr/graphics/untethered-device-optimization)). Succede: due fonti ufficiali, due consigli. Si prova sul visore e si misura. Lo vediamo nella lezione 10.
- **Lo studio.** Ti dirà quanta memoria hanno le lightmap del livello, quanti texel per metro, quante luci vive. È uno dei budget della guida di stile, come i triangoli e i materiali.

## 12. Problemi comuni
- **Il primo compito resta Da fare.** I modelli del faro non ci sono ancora: chiudi Unity, lancia `git aggiorna` un'altra volta e riapri. Unity Hub deve essere aperto.
- **Quello che hai cambiato è sparito.** L'hai cambiato in Play, e quando esci dal Play Unity rimette tutto com'era. Misuri in Play, curi fuori dal Play.
- **Il menu dei Display non risponde in Play.** Il rig ha preso il mouse: premi **Esc**.
- **Le cose che si muovono sono scure e piatte.** Non hanno light probe attorno, e prendono solo la luce del cielo. Aggiungi un **Light Probe Group** attorno a loro e rifai **Generate Lighting**.
- **Un oggetto reso statico dopo il bake, per esempio una statua, ha una luce strana, e la pagina resta Da fare.** Non ha ancora la sua lightmap: ogni statico nuovo vuole un bake nuovo. Rifai **Generate Lighting**.
- **Dopo il bake la stanza è troppo buia, o troppo chiara.** Cotta, la luce del cielo entra solo dalle finestre, e quella delle lampade rimbalza e si somma. Regola **Intensity** delle luci e ricuoci.
- **La luce filtra da un muro, o da un angolo.** Il muro è sottile, o l'angolo è aperto. Nel programma 3D: muri con lo spessore e angoli chiusi; nelle UV fatte a mano, margini più larghi. Unity aggiunge: con le UV generate, **Margin Method** su **Calculate**; la vista **UV Overlap** per trovare i texel che danno problemi; per le superfici sottili, **Double Sided Global Illumination** ([Unity, i difetti delle lightmap](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/troubleshooting/troubleshooting-lightmapping-artifacts)). Se la luce filtra sulle cose che si muovono: due probe per parte vicino al muro.
- **Il bake sulla scheda video non parte, o è lentissimo.** Controlla i requisiti: OpenCL 1.2, 2 GB di memoria video, SSE4.1. Chiudi gli altri programmi che usano la scheda video. Se non basta, metti **Lightmapper** su **Progressive CPU**: il processore è più lento, ma ci arriva.
- **Il bake dura troppo.** Abbassa **Lightmap Resolution** e i campioni, e togli gli oggetti piccoli dalle lightmap.
- **Le luci Baked si comportano come quelle in tempo reale.** **Baked Global Illumination** è spento. Senza un sistema di GI, Unity tratta le luci Baked e Mixed come Realtime.
- **La luce cotta è granulosa.** Pochi campioni: alza **Indirect Samples**, soprattutto con la brace e gli altri emissivi, e lascia acceso il filtro, **Filtering**.
- **La brace non illumina niente.** Nel suo materiale **Global Illumination** è ancora su **Realtime**, oppure `Brace` non è statica, oppure non hai rifatto il bake.
- **Lo specchio riflette ancora il cielo.** La reflection probe non è cotta, oppure il suo **Type** non è **Baked**, oppure la scatola è piccola e lo specchio ne resta fuori, oppure **Blend Distance** è ancora a 1: lo specchio sta nella fascia dove il riflesso sfuma nel cielo. Mettila a 0.1 e ricuoci.
- **I tuoi numeri sono diversi da quelli del compagno, e le lampade non fanno ombra nemmeno nel prima.** Il tuo editor è rimasto su Android dopo una build per il visore, e disegna col profilo del visore. **File ▸ Build Profiles**, **Windows**, **Switch Platform**: la pagina «L'editor sul PC» del tutorial lo controlla.
- **Sul visore i riflessi hanno delle cuciture.** Con la nostra versione di Unity, la 6000.3.25f1, le reflection probe con la **Box Projection** possono mostrare delle cuciture sui dispositivi mobili. Unity l'ha corretto in una versione successiva, la 6000.3.26f1 ([Unity, le note della versione 6000.3.26f1](https://unity.com/releases/editor/whats-new/6000.3.26f1)). Se le vedi sul visore, apri l'asset `Mobile_RPAsset` e, nel riquadro **Lighting**, togli la spunta a **Box Projection**.
- **Hai cambiato il cielo, e la stanza no.** Da Unity 6 la luce del cielo cambia solo col bake: rifai **Generate Lighting**.
- **Dopo Clear Baked Data la luce della stanza è cambiata.** Hai cancellato anche la luce del tramonto, che era cotta: rifai **Generate Lighting**.
- **Sullo scaffale c'è ancora l'ombra dei libri.** In Blender **Select Overlap** trova ancora facce sovrapposte: rifai **Lightmap Pack** con un **Margin** più alto. Oppure nel modello **Generate Lightmap UVs** è acceso, e Unity ha messo le sue UV al posto delle tue: togli la spunta e premi **Apply**.
- **Blender ha fatto una terza mappa UV.** **New UV Map** era acceso. Seleziona la mappa nuova, premi **−**, poi rifai **Lightmap Pack** sulla `Lightmap` con **New UV Map** spento.
- **Lo scaffale è minuscolo.** Hai esportato senza **Apply Scalings: FBX Units Scale**: rifai l'esportazione.
- **Blender ha esportato anche il cubo e la luce.** Mancava **Selected Objects**: rifai l'esportazione col solo scaffale selezionato.
- **Lo scaffale non cambia.** Hai salvato con un altro nome o in un'altra cartella: il file deve essere `scaffale.fbx`, in `sandbox/Assets/Demo/Models/Faro`.
- **Il compito resta Da fare, e la cura è giusta.** Nella stanza qualcosa è stato spostato, spento o cancellato: annulla con **Ctrl+Z**, o chiamami.
- **La foto è nera o vuota.** Premi di nuovo il pulsante: subito dopo aver aperto la scena, la prima foto può uscire senza oggetti.
- **Il faro è rovinato.** Cancella la cartella `Assets/Samples/RUFA Core/Lezione 08 - Luce` e premi di nuovo **Apri il faro**. Le cure fatte lì dentro si perdono; le foto e il file delle misure restano.

## Fonti
**Meta**
- Meta, «Lighting and effects»: cuocere la luce, le luci in tempo reale con parsimonia, le light probe, i materiali emissivi, i frame al secondo: https://developers.meta.com/horizon/design/lighting-and-effects/
- Meta, «Android Development», le regole per le app autonome: geometria statica con le lightmap, light probe per le cose che si muovono, lightmap cotte e non direzionali, dettagli nelle texture, niente luce dinamica, lo skybox: https://developers.meta.com/horizon/documentation/unity/unity-mobile-performance-intro/
- Meta, «Best practices for VR»: Forward e Forward+, niente GI in tempo reale, poche luci per pixel, ombre spente vicino ai limiti: https://developers.meta.com/horizon/documentation/unity/unity-best-practices-intro/
- Meta, «Forward+ Rendering»: quando conviene il Forward+, Probe Blending e Probe Atlas Blending: https://developers.meta.com/horizon/documentation/unity/unity-forward-plus-rendering/
- Meta, «Art Direction for All-in-One VR Performance»: le ombre dinamiche e la luce statica: https://developers.meta.com/horizon/documentation/unity/po-art-direction/
- Meta, «Balancing art and performance for immersive experiences»: https://developers.meta.com/horizon/design/art-and-performance/
- Meta, «Using RenderDoc Meta Fork to Optimize Your App - Part 1»: le draw call delle ombre, una reflection probe per gli oggetti da disegnare insieme: https://developers.meta.com/horizon/documentation/unity/po-renderdoc-optimizations-1/
- Meta, «Unity for VR Development - Settings and Features to Minimize Iteration Time»: niente ombre accese di base: https://developers.meta.com/horizon/resources/unity-settings-features-minimize-iteration/
- Meta, «Open World Games and Asset Streaming»: le lightmap e lo static batching, i LOD, la memoria, i triangoli con la sola luce cotta: https://developers.meta.com/horizon/documentation/unity/po-assetstreaming/
- Meta, i consigli della GDC sul design per la MR e la VR, con la luce di Batman: Arkham Shadow: https://developers.meta.com/horizon/blog/gdc-2025-design-strategies-considerations-tips-mr-vr-horizon-os/
- Meta, «Testing and performance analysis» per Unity: frame al secondo, draw call e triangoli per Quest 3 e 3S: https://developers.meta.com/horizon/documentation/unity/unity-perf/
- Meta, «Project Configuration Overview»: l'MSAA 4x: https://developers.meta.com/horizon/documentation/unity/unity-conf-settings/
- Meta, la nota tecnica vecchia sulle impostazioni di Unity per la VR mobile: https://developers.meta.com/horizon/blog/tech-note-unity-settings-for-mobile-vr/

**Unity**
- Unity 6.3, i modi della luce (Realtime, Mixed, Baked): https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/light-modes/introduction
- Unity 6.3, scegliere il modo della luce, la tabella dei costi: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/light-modes/choose
- Unity 6.3, il Lighting Mode e come impostarlo: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/lighting-mode/lighting-mode e https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/lighting-mode/light-mode-scene
- Unity 6.3, l'asset Lighting Settings: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-reference/class-lighting-settings
- Unity 6.3, la finestra Lighting (le schede, Generate Lighting, Bake On Scene Load): https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-reference/lighting-window
- Unity 6.3, cuocere la luce: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/bake
- Unity 6.3, le impostazioni del bake (Lightmapper, campioni, Max Bounces, Filtering, Lightmap Resolution, Max Lightmap Size, Lightmap Compression, Directional Mode): https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/lightmaps-reference
- Unity 6.3, le lightmap direzionali: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/configure-lightmapping-settings/directional
- Unity 6.3, il lightmapper sulla CPU o sulla GPU, coi requisiti: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/configure/progressive-lightmapper
- Unity 6.3, ottimizzare il bake: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/gpuprogressive-lightmapper
- Unity 6.3, i lightmapper e le UV che vogliono: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/lightmappers
- Unity 6.3, il formato delle lightmap (Lightmap Encoding, ASTC): https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmap-data/lightmaps-technical-information
- Unity 6.3, le novità della versione (il lightmapper sulla GPU di base): https://docs.unity.com/en-us/engine/6000.3/manual/whats-new/unity63
- Unity 6.3, la guida per passare a Unity 6 (niente Auto Generate, la luce del cielo col bake): https://docs.unity.com/en-us/engine/6000.3/manual/upgrade-guides/upgrade-guide-unity6
- Unity 6.3, gli oggetti statici: https://docs.unity.com/en-us/engine/6000.3/manual/working-with-gameobjects/gameobject-fundamentals/static-objects
- Unity 6.3, il Mesh Renderer (Contribute e Receive Global Illumination, Scale In Lightmap, Stitch Seams): https://docs.unity.com/en-us/engine/6000.3/manual/assets-and-media/asset-types/mesh/components-reference/class-mesh-renderer
- Unity 6.3, generare le UV della luce, e le regole per farle a mano: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/lighting-gi-uvs/generating-lightmapping-uvs
- Unity 6.3, le impostazioni delle UV della luce (Pack Margin, Margin Method): https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/lighting-gi-uvs/reference
- Unity 6.3, i difetti delle lightmap: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/troubleshooting/troubleshooting-lightmapping-artifacts
- Unity 6.3, le viste della luce nella Scene view (Baked Lightmap, UV Overlap, Contributors / Receivers, Lighting Data, Show Lightmap Resolution): https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-reference/givis
- Unity 6.3, gli overlay Draw Modes e Debug Mode: https://docs.unity.com/en-us/engine/6000.3/manual/working-with-scenes/scenes-manage-gameobjects/overlays/reference/draw-modes e https://docs.unity.com/en-us/engine/6000.3/manual/working-with-scenes/scenes-manage-gameobjects/overlays/reference/debug-mode
- Unity 6.3, le light probe: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/light-probes
- Unity 6.3, mettere le light probe e il Light Probe Group: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/class-light-probe-group e https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/reference
- Unity 6.3, i problemi delle light probe: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/troubleshooting
- Unity 6.3, le light probe e le cose che si muovono, Anchor Override: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/moving-objects e https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/mesh-renderer
- Unity 6.3, i Light Probe Proxy Volume, nella pipeline vecchia: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-birp/light-probe-proxy-volume/class-light-probe-proxy-volume
- Unity 6.3, le Adaptive Probe Volumes: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/probevolumes/concept e https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/probevolumes/use
- Unity 6.3, usare le reflection probe: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/reflections/using-reflection-probes
- Unity 6.3, il componente Reflection Probe: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/reflections/class-reflection-probe
- Unity 6.3, ottimizzare i riflessi: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/reflections/ref-probe-performance
- Unity 6.3, le reflection probe in URP: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/reflection-probes/introduction
- Unity 6.3, lo shader Lit (Emission, Global Illumination, Environment Reflections): https://docs.unity.com/en-us/engine/6000.3/manual/materials-and-shaders/built-in/shaders-in-universalrp/reference/lit-shader
- Unity 6.3, i materiali emissivi: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-birp/lighting-emissive-materials
- Unity 6.3, ottimizzare le ombre in URP: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/shadows-in-urp/shadows-optimization
- Unity 6.3, la risoluzione delle ombre in URP: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/shadows-in-urp/shadow-resolution-urp
- Unity 6.3, i limiti delle luci in URP: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/light-limits-in-urp
- Unity 6.3, l'asset di URP: https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/urp-reference/universalrp-asset
- Unity 6.3, configurare URP per le prestazioni: https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/introduction/configure-for-better-performance
- Unity 6.3, i percorsi di disegno di URP: https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/introduction/urp-concepts/rendering-paths/comparison
- Unity 6.3, il confronto fra le pipeline: https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/choose-a-render-pipeline/feature-comparison
- Unity 6.3, i Rendering Layers: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/rendering-layers/introduction e https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting/rendering-layers/lights
- Unity 6.3, i visori autonomi (MSAA): https://docs.unity.com/en-us/engine/6000.3/manual/xr/graphics/untethered-device-optimization
- Unity 6.3, il Light Explorer: https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/lighting-explorer/lighting-explorer
- Unity 6.3, la finestra Statistics: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/profiling/rendering-statistics
- Unity 6.3, il Frame Debugger: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/profile-rendering/frame-debugger/debug
- Unity 6.3, il modulo Rendering del Profiler: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/profile-rendering/profiler-rendering
- Unity 6.3, ridurre il lavoro di CPU e GPU: https://docs.unity.com/en-us/engine/6000.3/manual/analysis/graphics-performance-profiling/optimizing-graphics-performance
- Unity, le note della versione 6000.3.26f1 (le cuciture della Box Projection sui dispositivi mobili): https://unity.com/releases/editor/whats-new/6000.3.26f1

**Blender**
- Blender 4.5, gli operatori UV (Lightmap Pack, Smart UV Project): https://docs.blender.org/manual/en/4.5/modeling/meshes/editing/uv.html
- Blender 4.5, le UV map (la fotocamera, la mappa selezionata): https://docs.blender.org/manual/en/4.5/modeling/meshes/uv/uv_texture_spaces.html
- Blender 4.5, selezionare le UV (Select Overlap): https://docs.blender.org/manual/en/4.5/editors/uv/selecting.html
- Blender 4.5, importare ed esportare FBX: https://docs.blender.org/manual/en/4.5/addons/import_export/scene_fbx.html
