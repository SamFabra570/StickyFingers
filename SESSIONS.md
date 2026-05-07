# StickyFingers — Sesiones con Claudio

## Los Mandamientos

1. **Acento argentino** — Claudio nunca olvida de dónde es.
2. **Aprobación previa** — Antes de modificar código, Claudio pregunta y espera respuesta.
3. **Solo .cs del proyecto** — Claudio solo edita archivos `.cs` del proyecto. Puede leer cualquier otro. Archivos de sesión (como este) son excepción.
4. **Backlog siempre activo** — Al final de cada sesión se actualiza el backlog. Al inicio de cada sesión, Claudio lee este archivo ANTES de hacer cualquier cosa, recupera contexto de Engram, y arranca por "Próxima sesión empieza por".
5. **Un solo documento** — Mandamientos y backlog viven acá. No hay copias.
6. **Flujo de trabajo** — Claudio escribe el código y guía cómo implementarlo en el motor. El usuario lo aplica en Unity.
7. **Rama master** — Todo el trabajo va sobre `master`.
8. **Aviso de tokens** — Si Claudio detecta que se está quedando sin tokens, hace un resumen y avisa para retomar en otra sesión.

---

## Antes de arrancar cada sesión

1. Leer este archivo completo
2. Recuperar contexto de Engram (`mem_context` + `mem_search`)
3. Revisar la sección **⚠️ REVERTIR ANTES DE SHIP**
4. Arrancar por **Próxima sesión empieza por**

---

## Contexto del proyecto

- **Género**: Sigilo isométrico — el jugador entra a un nivel a robar objetos
- **Unity**: 6000.2.12f1 con URP 17.2.0
- **Input**: Unity Input System moderno
- **AI**: NavMesh + FSM triple (EnemyFSM / EnemyScout / EnemyMage)
- **Rama activa**: `master`
- **Escena de prueba**: `Game2` (temporal — ver ⚠️ abajo)

### Scripts clave

| Sistema | Archivos principales |
|---|---|
| Player | `PlayerController.cs`, `PlayerVision.cs`, `PlayerVisionCone.cs` |
| Enemigos | `BaseEnemy.cs`, `EnemyStateMachine.cs`, `BaseScoutEnemy.cs`, `BaseMageEnemy.cs` |
| Abilidades | `Ability.cs`, `AbilityManager.cs`, `AbilityState.cs` |
| Inventario | `InventorySystem.cs`, `InventoryItem.cs`, `ItemController.cs` |
| Managers | `GameManager.cs`, `UIManager.cs`, `TimeManager.cs` |
| Portal | `ExitPortal.cs`, `PortalSpawner.cs`, `PortalState.cs` |
| HUB | `PlanningDesk.cs`, `HUB_UIManager.cs` |
| ZoneInteractables | `IInteractable.cs`, `NoiseEmitter.cs` + ver tabla abajo |

### Integración ZoneInteractables (patrones clave)

- Tag `"Interactable"` + Collider trigger → `PlayerController` lo detecta y llama `IInteractable.Interact()`
- Tag `"Object"` + Collider → items robables vía `ItemController`
- Invisibilidad: `PlayerController.Instance.isInvisible = true/false` → `Sight.cs` lo respeta
- Sonido: `SoundPlayer.distance_` (hijo del player) → sprint=4f, walk=2f, quiet=0f
- Lluvia reduce radio de sprint: `RainController.Instance.IsRaining` → `PlayerController.UpdateSpeed()`
- Enemigos: `BaseEnemy.agent_`, `BaseScoutEnemy.agent_`, `BaseMageEnemy.agent_`
- `NoiseEmitter` usa `GetComponentInParent` para encontrar enemigos por jerarquía
- Código legacy en `Assets/Scripts/[OLD STUFF]/` — ignorar

---

## ⚠️ REVERTIR ANTES DE SHIP

- `GameManager.cs:44` — `StartGame()` apunta a `"Game2"` (temporal). Revertir a `"Game"` cuando todas las ZoneInteractables estén implementadas y probadas.

---

## Backlog — ZoneInteractables

### Implementadas ✅
| Mecánica | Código | En Unity |
|---|---|---|
| Infraestructura (`IInteractable`, `NoiseEmitter`) | ✅ | ✅ |
| Saqueo (`Chest`, `KeyItemData`) | ✅ | ✅ |
| Libro Sorpresa (`BookShelf`, `BookItemData`, `GrassPatch`, `RainController`, `DemonHelper`) | ✅ | ✅ |

### Pendientes ⬜
| Mecánica | Código | En Unity |
|---|---|---|
| Crea tu Poción | ✅ | ⬜ |
| Catering | ✅ | ⬜ |
| Que empiece la fiesta | ✅ | ⬜ |
| Curador de Arte | ✅ | ⬜ |
| Secret Vintage | ✅ | ⬜ |

---

## Próxima sesión empieza por

**Crea tu Poción** — setup en Unity:

1. `Create → Inventory → IngredientItemData` × 3: `Herb`, `Liquid`, `Powder`
2. GameObject `Cauldron`: Collider trigger + tag `Interactable` + `CauldronController.cs` + `ObjectSpawner.cs`
3. Hijo vacío `NoiseEmitter` en el caldero: `NoiseEmitter.cs`, radio `5`, asignarlo en `CauldronController`
4. Asignar `AudioSource` + clips (éxito y pedo) en `CauldronController`
5. Colocar 3 ingredientes en escena: Collider + tag `Object` + `ItemController` con SO correspondiente
6. GameObject `Poster`: Collider trigger + tag `Interactable` + `PosterHint.cs`
7. Crear panel UI con imagen de la pista (2 ingredientes + ?), asignarlo en `PosterHint → Hint Panel`

**Tests**:
- [ ] Poster → panel aparece y pausa / segundo F → cierra
- [ ] Herb + Liquid + Powder → poción spawea
- [ ] Combo incorrecto → pedo + guardias cercanos van al caldero

Luego continuar con **Catering**, **Que empiece la fiesta**, **Curador de Arte**, **Secret Vintage** en ese orden.

---

## Historial de sesiones

### Sesión 1 — 2026-05-04
Exploración inicial del repo, definición de mandamientos, diseño del plan de ZoneInteractables.

### Sesión 2 — 2026-05-04
Código de las 7 mecánicas escrito (22 scripts). Cambios en `PlayerController`.

### Sesión 3 — 2026-05-05 / 2026-05-07
- Fix error de compilación en `IInteractable.cs` (faltaba `using UnityEngine`)
- Implementado en Unity: Saqueo ✅, Libro Sorpresa ✅
- Bugs resueltos: grass patches en paredes/altura, lluvia no activaba, enemigos no detectaban al demonio
- `GameManager.cs` apunta temporalmente a `Game2`
