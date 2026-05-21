# OCR/RPR Validation Checklist (Issue #109)

## Escenario controlado
1. Ejecutar la misma escena/cámara por al menos 300 frames con OCR **desactivado** (`RenderSystem3D.SetOcclusionCullingEnabled(false)`).
2. Repetir exactamente la corrida con OCR **activado** (`RenderSystem3D.SetOcclusionCullingEnabled(true)`).
3. Comparar líneas de instrumentación por frame:
   - `[Render3D] DepthReadyBeforeOcrQuery=...`
   - `[OCR][Frame N] RPR=... OCR Visible=... Occluded=... PendingQueries=... IsOccludedTransitions=...`

## Criterios de aceptación medibles
- **Pass order/depth listo:** `DepthReadyBeforeOcrQuery=true` en >= 99% de los frames.
- **Diferencia funcional OCR OFF vs ON:**
  - OCR OFF: `OCR Visible == RPR` en >= 99% de los frames.
  - OCR ON: existe al menos un frame con `OCR Occluded > 0` en escena con oclusores.
- **Transiciones válidas de estado:** al menos una transición `false->true` y `true->false` para una entidad cuando atraviesa oclusión/visibilidad.
- **Estabilidad:** sin exceptions durante 300 frames en configuraciones `YTB`, `Debug`, `Release`.
- **Consistencia cross-config:** diferencia de conteos por frame entre configuraciones <= 1 entidad para la misma escena/cámara semilla.

## Registro recomendado
Guardar logs por configuración:
- `logs/ocr_ytb.log`
- `logs/ocr_debug.log`
- `logs/ocr_release.log`

y adjuntar tabla comparativa:
- promedio `RPR`
- promedio `OCR Visible`
- promedio `OCR Occluded`
- total de transiciones `IsOccluded`
