# sopra06

- [Sopra Wiki](https://sopranium.de)
- Mailingliste sopra06@informatik.uni-freiburg.de

## Gruppe

Marvin, Raphael, Tyler, Sinan, Frederik, Noah, Rojus

Gruppentreffen: Montag, 16:00 - 18:00, im SR 02-017 Geb. 052

- Product Owner: Noah / Frederik
- Architektur: Tyler    https://gitmind.com/app/docs/fsqvms2z
- Qualitätssicherung: NAME_HIER 

## Definition of Done

- Das Item ist in Gitea geschlossen.
- Im Item sind die geschätzte und die tatsächliche Arbeitszeit eingetragen.
- Alle für das Item relevanten Dateien sind im aktuellen Stand des remote release Branch integriert.
- Der Tutor hat die Fertigstellung des Items im Sprint Review anhand des aktuellen Standes des remote release Branch bestätigt.
- Sonntag Abend 20:00 Uhr wird ein Release vom PO gemacht, falls danach noch jemand etwas ändert ist derjenige verantwortlich dafür das ein neuer Release zur Verfügung steht und das dann alle Funktionalitäten aus dem Release von Sonntag Abend erhalten bleiben.

## Hinweise

- Laden Sie keine kompilierten Binärdateien hoch (z.B.: `*.exe`, `*.bin`). Ausname hier sind _nur_ die Archive für die Abgaben.
- Laden Sie keine benutzerspezifischen Dateien hoch (Zum Beispiel `*.suo`, `*.user`)
- Geben Sie jedem Commit eine aussagekräftige Nachricht. Verweisen Sie wenn möglich auf den Issue und sagen Sie was Sie geändert haben.
- Laden Sie keine Änderungen am Programm hoch, die nicht kompillieren (falls es nötig ist kaputten Code zu teilen, benutze einen `wip/<somename>`-branch)
- Ändern sie bei Dateinamen und Ordnern niemals nur die Groß/Kleinschreibug, da dies Git+Windows völlig verwirrt.


# SoPra Do's and Don'ts
## Disclaimer
- Dieser Teil wurde nicht von den Dozenten erstellt und ist möglicherweise nicht vollständig korrekt.
- Um genaue Informationen zu erhalten, schaut bitte im <a href="https://sopranium.de/Hauptseite">Wiki</a> nach.


## Grundlagen Sopra
- Attest / Bescheinigung, ansonsten zählt es als unentschuldigt.
- Der Tutor ist für die Form verantwortlich, um den Rest muss sich die Gruppe selbst kümmern.
- Issues zuweisen / Est nur im Meeting, mit Ausnahmen:
    - Zum Beispiel, wenn für ein Issue nur 30 Minuten statt der geschätzten 8 benötigt wurden, kann man die Gruppe gefragt, ob man ein bestimmtes Issue aus dem Backlog erledigen kann.
    - Bugs sollten gerne direkt behoben werden (Issue anlegen).

## Ablauf: Wie arbeitet man?
- Das zu bearbeitende Issue gründlich lesen und verstehen, was das gewünschte Ergebnis sein soll. Wenn nicht klar, nachfragen.
- Pullen (<a href="https://sopranium.de/GitWorkflow">GitWorkflow</a>)
- Arbeiten -> Fertigstellen, bis es funktioniert (keine ERRORs commiten / pushen).
    - Wenn man nicht weiterkommt, in der Gruppe nachfragen, ob jemand eine Idee hat, etc.
- Commiten, mit einer Nachricht, die klar zeigt, was sich geändert hat. (<a href="https://sopranium.de/GitWorkflow">GitWorkflow</a>)
- Pullen (<a href="https://sopranium.de/GitWorkflow">GitWorkflow</a>)
- Merge-Konflikte lösen. (<a href="https://sopranium.de/GitWorkflow">GitWorkflow</a>)
- Pushen. (<a href="https://sopranium.de/GitWorkflow">GitWorkflow</a>)
- Zeit im Issue loggen und, wenn nötig, einen Kommentar verfassen.
- Issue schließen.
- Issues, die daraus entstanden sind, erstellen (z.B. Bugs usw.).

## Wie erstellt man ein Issue:
- Klare Überschrift (nicht zu kompakt).
- Weitere Einzelheiten als Kommentar hinzufügen: Was sind die Kriterien, damit das Issue geschlossen werden kann?
- Noch keine Schätzung (Est), zuständige Person oder Meilenstein festlegen. Diese werden im Sprint-Meeting festgelegt (mit einigen Ausnahmen).
- Issues bilden das Backlog, was in der nächsten Zeit noch erledigt werden muss.
- Im Meeting werden Schätzungen (Est), zuständige Person und Meilenstein eingetragen.

## Issue nicht beendet bis zum Meeting?
- Die Person erhält Punktabzug, relativ zu den Schätzungen der anderen zu bearbeitenden Issues.
- Die Person wird vom Issue entfernt (diese soll trotzdem die Zeit loggen, bekommt aber das Geschätzte nicht gutgeschrieben).
- Das Issue wird damit zurück ins Backlog geschoben.
- Wann das Issue als erledigt gilt, ist manchmal eine Ermessensfrage.
    - Gruppenentscheidung:
        - Wenn die Gruppe der Meinung ist, dass eine Aufgabe nicht wie im Issue beschrieben erledigt wurde, kann sie als nicht bearbeitet gewertet werden.
    - Tutor-Entscheidung:
        - Keine erfasste Zeit.
        - Das Issue wurde nicht geschlossen.
        - Kein aussagekräftiger Kommentar bei Recherche oder Textaufgaben vorhanden.

## Product Owner
- Überblick über das Backlog und Meilensteine behalten.
- Jede Woche das Backlog füllen (basierend auf dem GDD) und Issues erstellen, die für den nächsten <a href="https://sopranium.de/Hauptseite">Meilenstein</a> relevant sind.
- Kurz vor dem Meeting:
    - Den Master auf den Release Mergen. (<a href="https://sopranium.de/Gitea">Gitea</a>)
    - Den Sprint-Meilenstein in Gitea schließen
    - Den neuesten Build herunterladen und für die Review bereithalten.
- Im Meeting:
    - Die Issues mit der Gruppe verteilen und schätzen.

## Developer
- Jeder geschriebene Code (bearbeitete Issues) kann innerhalb von 5 Minuten im Release-Build gezeigt und bewertet werden, ohne den Code zu ändern.
- Jeder darf Issues erstellen, aber sie werden zunächst in den Backlog aufgenommen.
