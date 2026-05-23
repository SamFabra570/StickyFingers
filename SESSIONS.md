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

## 🐛 Pendiente — Cono de visión magenta

Después del refactor de `VisionCone.cs` (que ahora usa `RequireComponent` en lugar de `AddComponent` en runtime), el material `Stun` (shader `ForceFieldShader.shadergraph`) aparece magenta en el cono.

**Hipótesis a verificar al retomar**:
1. Si la preview del material `Stun` en el inspector ya está magenta → el `ForceFieldShader.shadergraph` tiene un error de compilación. Abrir en Shader Graph y revisar.
2. Si la preview está bien pero el cono en Game View está magenta → el problema es que `VisionCone.DrawVisionCone()` solo setea `vertices` y `triangles`, NO genera UVs/normales/tangentes. El shader puede estar fallando por falta de esos vertex attributes.

**Test rápido para diferenciar**: asignar un material URP/Lit Transparent al cono. Si se ve bien → el problema es el shader. Si también se ve magenta → el problema es el mesh procedural.

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

## Mejoras de sigilo (Sesión 5) — código ✅, falta tunear en Unity

Las 8 mejoras de gameplay de sigilo están implementadas en código. Pendiente: setup y tuneo en el inspector.

| Fase | Mejora | Archivos | Falta en Unity |
|---|---|---|---|
| 0 | `DitherVisibility.IsVisible` expuesto (fundación) | `DitherVisibility.cs` | — |
| 1-2 | Velocidad oculta/visible (×3 enemigos) | `BaseEnemy/Scout/Mage.cs` | Setear `hiddenSpeed`/`visibleSpeed`; verificar `DitherVisibility` en Scout/Mage |
| 5-6 | Objetos visibles para siempre + linger en enemigos | `DitherVisibility.cs` | Tildar `staysVisibleOnceSeen` en objetos, `lingerTime` en enemigos |
| 3 | `lastKnownPlayerPosition` (×3 search/pursuit) | states de los 3 FSM | Subir `searchTime` en prefabs (10 → 15-20) |
| 4 | Cono visual del jugador: Light → mesh procedural | `PlayerVisionConeVisual.cs` (nuevo) | Sacar el Light, crear GO hijo con el componente + material |
| 5 (audio) | `EnemyProximityCue`: SFX cercanía + pulso procedural | `EnemyProximityCue.cs` (nuevo) | Agregar componente + AudioSource a los 3 prefabs |
| 6 | DemonHelper patrulla waypoints de guardias | `DemonHelper.cs` | — |

También se arregló un bug pre-existente: `DitherVisibility.SetVisible` crasheaba al iniciar corrutina en un objeto inactivo (el Scout se desactiva al spawnear el Mage).

---

## Backlog — Visión y detección (Sesión 6)

### Hechas en código ✅ (falta aplicar/tunear en Unity)
| # | Qué | Archivos | Falta en Unity |
|---|---|---|---|
| 1 | Fix oclusión: los enemigos ya NO detectan a través de paredes | `Sight.cs` | ⚠️ Verificar que `obstacles_layer_` incluya la layer de paredes en los 3 enemigos |
| 4 | Visión periférica (esfera de cercanía) enemigo + player | `Sight.cs`, `PlayerVisionCone.cs` | Tunear `peripheral_radius_` / `peripheralRadius` (default 2; 0 = off) |
| 3 | Warmup de detección del scout (no instantáneo) | `BaseScoutEnemy.cs`, `EnemyScoutPatrolState.cs` | Tunear `detectionWarmup` (default 1s) |
| 2 (limpieza) | Borrado `PlayerVision.cs` (código muerto, cero refs) | — | Chequear "missing script" en prefab/escena del player |

### Pendientes de reproducir 🟡 (NO tocar código sin repro)
- **Seguís viendo guardias que ya no deberían**: chequear primero `lingerTime` / `staysVisibleOnceSeen` en el inspector antes de tocar código.
- **Visión del player rara con colliders al acercarse**: necesita repro (clip o pasos) para diagnosticar.

### Otros clusters (Sesión 6, sin empezar)
- **Limpieza de contenido**: sacar chests si no hacen nada; dejar solo mecánicas entendibles; prototipos visuales (topo→agujero, manos abierta→cerrada al agarrar).
- **Atmósfera (Unity)**: fog (URP fog / Volume), post-processing (Volume + URP profile).
- **SFX**: footsteps, robar/soltar, wings/scout, guard, portal (hooks en código + clips del usuario).

---

## Próxima sesión empieza por

**Ya mismo**: aplicar y probar los 4 fixes de visión de Sesión 6 (ver tabla arriba). Crítico: confirmar `obstacles_layer_` con las paredes, y reproducir los 2 bugs 🟡.

**Cero**: terminar de tunear las mejoras de sigilo en Unity (ver tabla arriba) si quedó algo pendiente.

**Primero**: resolver el cono de visión magenta (ver sección 🐛 arriba). Hacer el test del material URP/Lit Transparent para diagnosticar.

**Después**: continuar con **Crea tu Poción** — setup en Unity:

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

### Sesión 4 — 2026-05-10
- Refactor `DitherVisibility.cs`: separa renderers en `_ditherRenderers` (con `_Base_Color` → fade alpha) y `_toggleRenderers` (sin esa propiedad → enable/disable). Fix error de `ParticlesUnlit` y soporte para mesh del cono de visión.
- Fix posicionamiento de `DitherVisibility`: va en el root del prefab Guard (no en los hijos de mesh) — `GetComponentInParent` sube, no baja.
- Refactor `VisionCone.cs`: ahora usa `[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]` y `GetComponent` en `Awake` en lugar de `AddComponent` en runtime. Esto permite que `DitherVisibility.Awake` encuentre el renderer del cono. Se eliminó el campo `VisionConeMaterial` — el material se asigna directo en el `MeshRenderer` del prefab.
- Pendiente para próxima sesión: cono magenta (ver sección 🐛).

### Sesión 6 — 2026-05-23
- Diagnóstico del sistema de visión. Arquitectura: los 3 enemigos detectan con el sensor compartido `Sight.cs`; el player revela con `PlayerVisionCone.cs` → `DitherVisibility`. `VisionCone.cs` y `PlayerVisionConeVisual.cs` son solo visuales. `PlayerVision.cs` era código muerto.
- **Bug oclusión (raíz encontrada)**: `Sight.cs` chequeaba la pared con `Linecast` pero el `else` detectaba IGUAL. Fix: si hay pared → no detecta. Arregla los 3 enemigos de una.
- **Visión periférica**: esfera de cercanía en `Sight.cs` (`peripheral_radius_`) y `PlayerVisionCone.cs` (`peripheralRadius`) — detecta/revela cosas muy pegadas aunque estén fuera del ángulo, sin atravesar paredes.
- **Warmup del scout**: `detectionWarmup` + `AccumulateSuspicion()` en `BaseScoutEnemy`; `EnemyScoutPatrolState` acumula sospecha mientras te ve (decae al perderte). Search sigue instantáneo a propósito (ya alertado).
- Borrado `PlayerVision.cs` + `.meta` (código muerto).
- Pendientes de reproducir: "seguís viendo guardias" (chequear inspector primero) y "visión rara con colliders".

### Sesión 5 — 2026-05-17
- Analizados 4 commits de otro dev: la Sesión 4 quedó commiteada en `8fc1e7c`; `GameManager` ya carga `Game` (no `Game2`); sistema de Stun nuevo; `AlchemyRoom.prefab` es solo geometría.
- ZoneInteractables EN PAUSA. Se priorizaron 8 mejoras de gameplay de sigilo — las 6 fases implementadas (ver sección "Mejoras de sigilo" arriba).
- Bugs arreglados: crash de corrutina en `DitherVisibility` (objeto inactivo); falta de `return` tras `ChangeState` en los 3 search states.
- Aclarada la mecánica del Scout: te ve → `EnemyScoutAttackState` → `MageSpawner.SpawnMage()` + `SetActive(false)`. Alerta al Mage sin perseguir. Se revirtió un cambio mío incorrecto que lo hacía perseguir.
