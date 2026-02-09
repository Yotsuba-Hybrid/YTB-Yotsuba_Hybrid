using System;
using System.Collections.Generic;
using System.Text;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    /// <summary>
    /// Entity actions mapped to input bindings.
    /// Acciones de la entidad, para mapear con uno o varios inputs 
    /// </summary>
    public enum ActionEntityInput
    {
        /// <summary>
        /// Move up action.
        /// Acción de moverse hacia arriba.
        /// </summary>
        MoveUp,
        /// <summary>
        /// Move down action.
        /// Acción de moverse hacia abajo.
        /// </summary>
        MoveDown,
        /// <summary>
        /// Move left action.
        /// Acción de moverse hacia la izquierda.
        /// </summary>
        MoveLeft,
        /// <summary>
        /// Move right action.
        /// Acción de moverse hacia la derecha.
        /// </summary>
        MoveRight,
        /// <summary>
        /// Attack action.
        /// Acción de atacar.
        /// </summary>
        Attack,
        /// <summary>
        /// Jump action.
        /// Acción de saltar.
        /// </summary>
        Jump,
        /// <summary>
        /// Dash action.
        /// Acción de dash.
        /// </summary>
        Dash,

        // --- Camera Actions (Completed from your list) ---
        // --- Acciones de Cámara (Completando tu lista) ---

        /// <summary>
        /// Rotate camera left action.
        /// Acción de rotar la cámara a la izquierda.
        /// </summary>
        RotateCameraLeft,

        /// <summary>
        /// Rotate camera right action.
        /// Acción de rotar la cámara a la derecha.
        /// </summary>
        RotateCameraRight,

        /// <summary>
        /// Rotate camera up action.
        /// Acción de rotar la cámara hacia arriba.
        /// </summary>
        RotateCameraUp,

        /// <summary>
        /// Rotate camera down action.
        /// Acción de rotar la cámara hacia abajo.
        /// </summary>
        RotateCameraDown,

        /// <summary>
        /// Zoom camera in.
        /// Acción de acercar la cámara (Zoom in).
        /// </summary>
        CameraZoomIn,

        /// <summary>
        /// Zoom camera out.
        /// Acción de alejar la cámara (Zoom out).
        /// </summary>
        CameraZoomOut,

        /// <summary>
        /// Reset camera to default position.
        /// Acción de reiniciar la cámara a su posición por defecto.
        /// </summary>
        CameraReset,

        /// <summary>
        /// Lock camera on target.
        /// Acción de fijar la cámara en el objetivo.
        /// </summary>
        CameraLockOn,

        /// <summary>
        /// Toggle between first and third person view.
        /// Alternar entre vista de primera y tercera persona.
        /// </summary>
        TogglePerspective,

        /// <summary>
        /// Look behind action.
        /// Acción de mirar hacia atrás.
        /// </summary>
        LookBehind,

        // --- Advanced Movement ---
        // --- Movimiento Avanzado ---

        /// <summary>
        /// Sprint action.
        /// Acción de correr o esprintar.
        /// </summary>
        Sprint,

        /// <summary>
        /// Toggle walking mode.
        /// Acción de alternar modo caminar/correr.
        /// </summary>
        WalkToggle,

        /// <summary>
        /// Crouch action.
        /// Acción de agacharse.
        /// </summary>
        Crouch,

        /// <summary>
        /// Prone action (lie down).
        /// Acción de tumbarse o cuerpo a tierra.
        /// </summary>
        Prone,

        /// <summary>
        /// Slide action.
        /// Acción de deslizarse por el suelo.
        /// </summary>
        Slide,

        /// <summary>
        /// Wall run action.
        /// Acción de correr por la pared.
        /// </summary>
        WallRun,

        /// <summary>
        /// Climb up action.
        /// Acción de escalar hacia arriba.
        /// </summary>
        ClimbUp,

        /// <summary>
        /// Climb down action.
        /// Acción de descender escalando.
        /// </summary>
        ClimbDown,

        /// <summary>
        /// Swim up / Surface.
        /// Acción de nadar hacia arriba o salir a la superficie.
        /// </summary>
        SwimUp,

        /// <summary>
        /// Dive / Swim down.
        /// Acción de bucear o nadar hacia abajo.
        /// </summary>
        SwimDown,

        /// <summary>
        /// Glide action.
        /// Acción de planear en el aire.
        /// </summary>
        Glide,

        /// <summary>
        /// Dodge roll action.
        /// Acción de rodar para esquivar.
        /// </summary>
        DodgeRoll,

        /// <summary>
        /// Grappling hook action.
        /// Acción de usar el gancho de agarre.
        /// </summary>
        Grapple,

        // --- Combat & Weapons ---
        // --- Combate y Armas ---

        /// <summary>
        /// Primary fire action.
        /// Disparo principal.
        /// </summary>
        PrimaryFire,

        /// <summary>
        /// Secondary fire action.
        /// Disparo secundario.
        /// </summary>
        SecondaryFire,

        /// <summary>
        /// Reload weapon.
        /// Recargar arma.
        /// </summary>
        Reload,

        /// <summary>
        /// Heavy attack action.
        /// Ataque pesado.
        /// </summary>
        HeavyAttack,

        /// <summary>
        /// Block or defend action.
        /// Acción de bloquear o defender.
        /// </summary>
        Block,

        /// <summary>
        /// Parry action.
        /// Acción de parry (desvío).
        /// </summary>
        Parry,

        /// <summary>
        /// Kick action.
        /// Acción de patear.
        /// </summary>
        Kick,

        /// <summary>
        /// Aim down sights (ADS).
        /// Acción de apuntar con la mira.
        /// </summary>
        AimDownSights,

        /// <summary>
        /// Throw grenade.
        /// Lanzar granada.
        /// </summary>
        ThrowGrenade,

        /// <summary>
        /// Switch to next weapon.
        /// Cambiar a la siguiente arma.
        /// </summary>
        NextWeapon,

        /// <summary>
        /// Switch to previous weapon.
        /// Cambiar al arma anterior.
        /// </summary>
        PreviousWeapon,

        /// <summary>
        /// Holster or unholster weapon.
        /// Enfundar o desenfundar arma.
        /// </summary>
        HolsterWeapon,

        /// <summary>
        /// Inspect current weapon.
        /// Inspeccionar el arma actual.
        /// </summary>
        InspectWeapon,

        /// <summary>
        /// Change firing mode (Single, Burst, Auto).
        /// Cambiar modo de disparo (Único, Ráfaga, Auto).
        /// </summary>
        ChangeFireMode,

        /// <summary>
        /// Equip weapon in slot 1.
        /// Equipar arma de la ranura 1.
        /// </summary>
        EquipSlot1,

        /// <summary>
        /// Equip weapon in slot 2.
        /// Equipar arma de la ranura 2.
        /// </summary>
        EquipSlot2,

        /// <summary>
        /// Equip weapon in slot 3.
        /// Equipar arma de la ranura 3.
        /// </summary>
        EquipSlot3,

        /// <summary>
        /// Equip weapon in slot 4.
        /// Equipar arma de la ranura 4.
        /// </summary>
        EquipSlot4,

        /// <summary>
        /// Use melee execution / finisher.
        /// Usar ejecución cuerpo a cuerpo / remate.
        /// </summary>
        Execution,

        // --- Interaction & Items ---
        // --- Interacción y Objetos ---

        /// <summary>
        /// General interaction (Talk, Open, etc.).
        /// Interacción general (Hablar, Abrir, etc.).
        /// </summary>
        Interact,

        /// <summary>
        /// Pick up item.
        /// Recoger objeto.
        /// </summary>
        PickUp,

        /// <summary>
        /// Drop current item.
        /// Soltar objeto actual.
        /// </summary>
        DropItem,

        /// <summary>
        /// Use currently selected item.
        /// Usar el objeto seleccionado actualmente.
        /// </summary>
        UseItem,

        /// <summary>
        /// Use health potion / medkit.
        /// Usar poción de salud / botiquín.
        /// </summary>
        Heal,

        /// <summary>
        /// Use mana potion / energy pack.
        /// Usar poción de maná / paquete de energía.
        /// </summary>
        RestoreEnergy,

        /// <summary>
        /// Toggle flashlight.
        /// Alternar linterna.
        /// </summary>
        Flashlight,

        /// <summary>
        /// Toggle night vision.
        /// Alternar visión nocturna.
        /// </summary>
        NightVision,

        /// <summary>
        /// Scan environment.
        /// Escanear el entorno.
        /// </summary>
        Scan,

        // --- Skills & Magic ---
        // --- Habilidades y Magia ---

        /// <summary>
        /// Activate Skill 1.
        /// Activar Habilidad 1.
        /// </summary>
        Skill1,

        /// <summary>
        /// Activate Skill 2.
        /// Activar Habilidad 2.
        /// </summary>
        Skill2,

        /// <summary>
        /// Activate Skill 3.
        /// Activar Habilidad 3.
        /// </summary>
        Skill3,

        /// <summary>
        /// Activate Skill 4.
        /// Activar Habilidad 4.
        /// </summary>
        Skill4,

        /// <summary>
        /// Activate Ultimate Ability.
        /// Activar Habilidad Definitiva (Ultimate).
        /// </summary>
        UltimateAbility,

        /// <summary>
        /// Cast selected spell.
        /// Lanzar hechizo seleccionado.
        /// </summary>
        CastSpell,

        /// <summary>
        /// Cancel current casting.
        /// Cancelar lanzamiento actual.
        /// </summary>
        CancelCast,

        /// <summary>
        /// Summon mount or vehicle.
        /// Invocar montura o vehículo.
        /// </summary>
        SummonMount,

        // --- UI & System ---
        // --- Interfaz y Sistema ---

        /// <summary>
        /// Toggle Pause Menu.
        /// Alternar Menú de Pausa.
        /// </summary>
        PauseMenu,

        /// <summary>
        /// Open Inventory.
        /// Abrir Inventario.
        /// </summary>
        OpenInventory,

        /// <summary>
        /// Open Map.
        /// Abrir Mapa.
        /// </summary>
        OpenMap,

        /// <summary>
        /// Open Quest Log / Journal.
        /// Abrir Diario de Misiones.
        /// </summary>
        OpenJournal,

        /// <summary>
        /// Open Skill Tree.
        /// Abrir Árbol de Habilidades.
        /// </summary>
        OpenSkillTree,

        /// <summary>
        /// Open Character Sheet / Stats.
        /// Abrir Ficha de Personaje / Estadísticas.
        /// </summary>
        OpenCharacterSheet,

        /// <summary>
        /// UI Submit / Accept.
        /// UI Aceptar / Confirmar.
        /// </summary>
        UISubmit,

        /// <summary>
        /// UI Cancel / Back.
        /// UI Cancelar / Atrás.
        /// </summary>
        UICancel,

        /// <summary>
        /// Quick Save game.
        /// Guardado Rápido.
        /// </summary>
        QuickSave,

        /// <summary>
        /// Quick Load game.
        /// Carga Rápida.
        /// </summary>
        QuickLoad,

        /// <summary>
        /// Take screenshot.
        /// Tomar captura de pantalla.
        /// </summary>
        Screenshot,

        /// <summary>
        /// Toggle HUD visibility.
        /// Alternar visibilidad del HUD.
        /// </summary>
        ToggleHUD,

        // --- Vehicle Controls ---
        // --- Controles de Vehículos ---

        /// <summary>
        /// Vehicle Accelerate / Throttle.
        /// Acelerar vehículo.
        /// </summary>
        VehicleAccelerate,

        /// <summary>
        /// Vehicle Brake / Reverse.
        /// Frenar vehículo / Marcha atrás.
        /// </summary>
        VehicleBrake,

        /// <summary>
        /// Vehicle Turn Left.
        /// Girar vehículo a la izquierda.
        /// </summary>
        VehicleTurnLeft,

        /// <summary>
        /// Vehicle Turn Right.
        /// Girar vehículo a la derecha.
        /// </summary>
        VehicleTurnRight,

        /// <summary>
        /// Vehicle Handbrake / Drift.
        /// Freno de mano / Derrape.
        /// </summary>
        VehicleHandbrake,

        /// <summary>
        /// Vehicle Boost / Nitro.
        /// Turbo / Nitro del vehículo.
        /// </summary>
        VehicleBoost,

        /// <summary>
        /// Use Vehicle Horn.
        /// Usar bocina (claxon).
        /// </summary>
        VehicleHorn,

        /// <summary>
        /// Toggle Vehicle Lights.
        /// Alternar luces del vehículo.
        /// </summary>
        VehicleLights,

        /// <summary>
        /// Exit Vehicle.
        /// Salir del vehículo.
        /// </summary>
        VehicleExit,

        /// <summary>
        /// Pitch vehicle nose up (Air/Water).
        /// Inclinar morro arriba (Aire/Agua).
        /// </summary>
        VehiclePitchUp,

        /// <summary>
        /// Pitch vehicle nose down (Air/Water).
        /// Inclinar morro abajo (Aire/Agua).
        /// </summary>
        VehiclePitchDown,

        // --- Multiplayer & Social ---
        // --- Multijugador y Social ---

        /// <summary>
        /// Push to talk.
        /// Pulsar para hablar.
        /// </summary>
        PushToTalk,

        /// <summary>
        /// Open Text Chat.
        /// Abrir chat de texto.
        /// </summary>
        OpenChat,

        /// <summary>
        /// Show Scoreboard.
        /// Mostrar tabla de puntuaciones.
        /// </summary>
        ShowScoreboard,

        /// <summary>
        /// Mark/Ping location contextually.
        /// Marcar ubicación contextualmente (Ping).
        /// </summary>
        PingLocation,

        /// <summary>
        /// Mark/Ping enemy.
        /// Marcar enemigo.
        /// </summary>
        PingEnemy,

        /// <summary>
        /// Emote: Wave.
        /// Emote: Saludar.
        /// </summary>
        EmoteWave,

        /// <summary>
        /// Emote: Dance.
        /// Emote: Bailar.
        /// </summary>
        EmoteDance,

        /// <summary>
        /// Emote: Taunt.
        /// Emote: Burlarse.
        /// </summary>
        EmoteTaunt,

        /// <summary>
        /// Vote Yes / Agree.
        /// Votar Sí / Estar de acuerdo.
        /// </summary>
        VoteYes,

        /// <summary>
        /// Vote No / Disagree.
        /// Votar No / No estar de acuerdo.
        /// </summary>
        VoteNo,

        // --- Building & Strategy (RTS/Builder Context) ---
        // --- Construcción y Estrategia ---

        /// <summary>
        /// Rotate placement object left.
        /// Rotar objeto a colocar a la izquierda.
        /// </summary>
        RotateObjectLeft,

        /// <summary>
        /// Rotate placement object right.
        /// Rotar objeto a colocar a la derecha.
        /// </summary>
        RotateObjectRight,

        /// <summary>
        /// Confirm placement / build.
        /// Confirmar colocación / construir.
        /// </summary>
        BuildConfirm,

        /// <summary>
        /// Cancel placement.
        /// Cancelar colocación.
        /// </summary>
        BuildCancel,

        /// <summary>
        /// Select all units.
        /// Seleccionar todas las unidades.
        /// </summary>
        SelectAllUnits,

        /// <summary>
        /// Command: Move to.
        /// Orden: Mover a.
        /// </summary>
        CommandMove,

        /// <summary>
        /// Command: Attack target.
        /// Orden: Atacar objetivo.
        /// </summary>
        CommandAttack,

        /// <summary>
        /// Command: Hold position.
        /// Orden: Mantener posición.
        /// </summary>
        CommandHold,

        // --- Debug & Developer ---
        // --- Depuración y Desarrollador ---

        /// <summary>
        /// Toggle Debug Console.
        /// Alternar consola de depuración.
        /// </summary>
        ToggleConsole,

        /// <summary>
        /// Toggle God Mode.
        /// Alternar Modo Dios.
        /// </summary>
        ToggleGodMode,

        /// <summary>
        /// Toggle NoClip mode.
        /// Alternar modo NoClip (atravesar paredes).
        /// </summary>
        ToggleNoClip,

        /// <summary>
        /// Slow down game time.
        /// Ralentizar tiempo de juego.
        /// </summary>
        TimeSlow,

        /// <summary>
        /// Speed up game time.
        /// Acelerar tiempo de juego.
        /// </summary>
        TimeSpeedUp,
        // --- Advanced Interaction / Puzzle ---
        // --- Interacción Avanzada / Puzles ---

        /// <summary>
        /// Examine detailed view of an object.
        /// Examinar vista detallada de un objeto.
        /// </summary>
        ExamineDetail,

        /// <summary>
        /// Push object forward.
        /// Empujar objeto hacia adelante.
        /// </summary>
        PushObject,

        /// <summary>
        /// Pull object backward.
        /// Tirar del objeto hacia atrás.
        /// </summary>
        PullObject,

        /// <summary>
        /// Ignite torch or light source.
        /// Encender antorcha o fuente de luz.
        /// </summary>
        Ignite,

        /// <summary>
        /// Extinguish fire or light source.
        /// Apagar fuego o fuente de luz.
        /// </summary>
        Extinguish,

        /// <summary>
        /// Hack terminal or computer.
        /// Hackear terminal o computadora.
        /// </summary>
        Hack,

        /// <summary>
        /// Pick lock.
        /// Ganzuar cerradura.
        /// </summary>
        PickLock,

        /// <summary>
        /// Combine items in crafting menu.
        /// Combinar objetos en el menú de crafteo.
        /// </summary>
        CombineItems,

        /// <summary>
        /// Disassemble item for parts.
        /// Desmontar objeto para piezas.
        /// </summary>
        Disassemble,

        /// <summary>
        /// Rotate puzzle piece clockwise.
        /// Rotar pieza de puzle en sentido horario.
        /// </summary>
        PuzzleRotateCW,

        /// <summary>
        /// Rotate puzzle piece counter-clockwise.
        /// Rotar pieza de puzle en sentido antihorario.
        /// </summary>
        PuzzleRotateCCW,

        // --- Advanced Inventory & UI Management ---
        // --- Gestión Avanzada de Inventario e UI ---

        /// <summary>
        /// Move item to stash/chest.
        /// Mover objeto al alijo/cofre.
        /// </summary>
        MoveToStash,

        /// <summary>
        /// Take all items from container.
        /// Tomar todos los objetos del contenedor.
        /// </summary>
        TakeAll,

        /// <summary>
        /// Split item stack.
        /// Dividir pila de objetos.
        /// </summary>
        SplitStack,

        /// <summary>
        /// Mark item as junk/favorite.
        /// Marcar objeto como basura/favorito.
        /// </summary>
        MarkItem,

        /// <summary>
        /// Compare item with equipped.
        /// Comparar objeto con el equipado.
        /// </summary>
        CompareItem,

        /// <summary>
        /// Sort inventory by name.
        /// Ordenar inventario por nombre.
        /// </summary>
        SortByName,

        /// <summary>
        /// Sort inventory by weight/value.
        /// Ordenar inventario por peso/valor.
        /// </summary>
        SortByValue,

        /// <summary>
        /// Toggle mini-map size (small/large).
        /// Alternar tamaño del mini-mapa.
        /// </summary>
        ToggleMiniMapSize,

        /// <summary>
        /// Pin quest to HUD.
        /// Fijar misión en el HUD.
        /// </summary>
        PinQuest,

        /// <summary>
        /// Read next page (book/note).
        /// Leer página siguiente.
        /// </summary>
        ReadNextPage,

        /// <summary>
        /// Read previous page (book/note).
        /// Leer página anterior.
        /// </summary>
        ReadPrevPage,

        // --- Flight & Space Sim (6DOF) ---
        // --- Simulación de Vuelo y Espacio (6DOF) ---

        /// <summary>
        /// Yaw left (Flight).
        /// Guiñada a la izquierda (Vuelo).
        /// </summary>
        FlightYawLeft,

        /// <summary>
        /// Yaw right (Flight).
        /// Guiñada a la derecha (Vuelo).
        /// </summary>
        FlightYawRight,

        /// <summary>
        /// Roll left (Flight).
        /// Alabeo a la izquierda (Vuelo).
        /// </summary>
        FlightRollLeft,

        /// <summary>
        /// Roll right (Flight).
        /// Alabeo a la derecha (Vuelo).
        /// </summary>
        FlightRollRight,

        /// <summary>
        /// Pitch up (Flight).
        /// Cabeceo arriba (Vuelo).
        /// </summary>
        FlightPitchUp,

        /// <summary>
        /// Pitch down (Flight).
        /// Cabeceo abajo (Vuelo).
        /// </summary>
        FlightPitchDown,

        /// <summary>
        /// Vertical thrust up (VTOL/Space).
        /// Empuje vertical arriba.
        /// </summary>
        ThrustUp,

        /// <summary>
        /// Vertical thrust down (VTOL/Space).
        /// Empuje vertical abajo.
        /// </summary>
        ThrustDown,

        /// <summary>
        /// Lateral thrust left (Space).
        /// Empuje lateral izquierdo.
        /// </summary>
        ThrustLeft,

        /// <summary>
        /// Lateral thrust right (Space).
        /// Empuje lateral derecho.
        /// </summary>
        ThrustRight,

        /// <summary>
        /// Toggle landing gear.
        /// Alternar tren de aterrizaje.
        /// </summary>
        ToggleLandingGear,

        /// <summary>
        /// Engage warp drive / hyperdrive.
        /// Activar motor de curvatura / hiperimpulsor.
        /// </summary>
        EngageWarp,

        /// <summary>
        /// Deploy countermeasures (flares/chaff).
        /// Desplegar contramedidas (bengalas).
        /// </summary>
        DeployCountermeasures,

        /// <summary>
        /// Target nearest enemy.
        /// Fijar enemigo más cercano.
        /// </summary>
        TargetNearestEnemy,

        /// <summary>
        /// Cycle hostile targets.
        /// Ciclar objetivos hostiles.
        /// </summary>
        CycleTargets,

        /// <summary>
        /// Cycle sub-systems targeting (engines, weapons).
        /// Ciclar subsistemas del objetivo (motores, armas).
        /// </summary>
        CycleSubsystems,

        // --- Squad & Companion Command ---
        // --- Comandos de Escuadrón y Compañeros ---

        /// <summary>
        /// Command squad: Follow me.
        /// Orden: Síganme.
        /// </summary>
        SquadFollow,

        /// <summary>
        /// Command squad: Hold fire.
        /// Orden: Alto el fuego.
        /// </summary>
        SquadHoldFire,

        /// <summary>
        /// Command squad: Open fire.
        /// Orden: Fuego a discreción.
        /// </summary>
        SquadOpenFire,

        /// <summary>
        /// Command squad: Go to point.
        /// Orden: Ir al punto.
        /// </summary>
        SquadGoTo,

        /// <summary>
        /// Command squad: Regroup.
        /// Orden: Reagruparse.
        /// </summary>
        SquadRegroup,

        /// <summary>
        /// Command squad: Flank left.
        /// Orden: Flanquear izquierda.
        /// </summary>
        SquadFlankLeft,

        /// <summary>
        /// Command squad: Flank right.
        /// Orden: Flanquear derecha.
        /// </summary>
        SquadFlankRight,

        /// <summary>
        /// Command squad: Take cover.
        /// Orden: Cubrirse.
        /// </summary>
        SquadCover,

        /// <summary>
        /// Command companion: Use special ability.
        /// Orden compañero: Usar habilidad especial.
        /// </summary>
        CompanionSpecial,

        /// <summary>
        /// Swap control to next character.
        /// Cambiar control al siguiente personaje.
        /// </summary>
        SwapCharacterNext,

        /// <summary>
        /// Swap control to previous character.
        /// Cambiar control al personaje anterior.
        /// </summary>
        SwapCharacterPrev,

        // --- Editor / Photo Mode / Creative ---
        // --- Modo Editor / Foto / Creativo ---

        /// <summary>
        /// Toggle Photo Mode.
        /// Alternar Modo Foto.
        /// </summary>
        TogglePhotoMode,

        /// <summary>
        /// Increase Field of View (FOV).
        /// Aumentar campo de visión (FOV).
        /// </summary>
        IncreaseFOV,

        /// <summary>
        /// Decrease Field of View (FOV).
        /// Disminuir campo de visión (FOV).
        /// </summary>
        DecreaseFOV,

        /// <summary>
        /// Increase Depth of Field (DOF).
        /// Aumentar profundidad de campo.
        /// </summary>
        IncreaseDOF,

        /// <summary>
        /// Decrease Depth of Field (DOF).
        /// Disminuir profundidad de campo.
        /// </summary>
        DecreaseDOF,

        /// <summary>
        /// Toggle Grid Snapping.
        /// Alternar ajuste a la rejilla.
        /// </summary>
        ToggleGridSnap,

        /// <summary>
        /// Duplicate selected object.
        /// Duplicar objeto seleccionado.
        /// </summary>
        DuplicateObject,

        /// <summary>
        /// Delete selected object.
        /// Borrar objeto seleccionado.
        /// </summary>
        DeleteObject,

        /// <summary>
        /// Undo last action (Editor).
        /// Deshacer última acción.
        /// </summary>
        UndoAction,

        /// <summary>
        /// Redo last action (Editor).
        /// Rehacer última acción.
        /// </summary>
        RedoAction,

        /// <summary>
        /// Save schematic/blueprint.
        /// Guardar esquema/plano.
        /// </summary>
        SaveBlueprint,

        /// <summary>
        /// Load schematic/blueprint.
        /// Cargar esquema/plano.
        /// </summary>
        LoadBlueprint,

        // --- VR (Virtual Reality) Specific ---
        // --- Específicas de VR ---

        /// <summary>
        /// Recenter VR View.
        /// Recentrar vista VR.
        /// </summary>
        VRRecenter,

        /// <summary>
        /// Teleport (VR Locomotion).
        /// Teletransportarse (Locomoción VR).
        /// </summary>
        VRTeleport,

        /// <summary>
        /// Grip Left Hand.
        /// Agarrar con mano izquierda.
        /// </summary>
        VRGripLeft,

        /// <summary>
        /// Grip Right Hand.
        /// Agarrar con mano derecha.
        /// </summary>
        VRGripRight,

        /// <summary>
        /// Trigger Left Hand.
        /// Gatillo mano izquierda.
        /// </summary>
        VRTriggerLeft,

        /// <summary>
        /// Trigger Right Hand.
        /// Gatillo mano derecha.
        /// </summary>
        VRTriggerRight,

        /// <summary>
        /// Snap Turn Left (VR).
        /// Giro rápido izquierda (VR).
        /// </summary>
        VRSnapTurnLeft,

        /// <summary>
        /// Snap Turn Right (VR).
        /// Giro rápido derecha (VR).
        /// </summary>
        VRSnapTurnRight,

        // --- Social / Chat / Emotes ---
        // --- Social / Chat / Emoticonos ---

        /// <summary>
        /// Mute all players.
        /// Silenciar a todos los jugadores.
        /// </summary>
        MuteAll,

        /// <summary>
        /// Unmute all players.
        /// Reactivar audio de todos.
        /// </summary>
        UnmuteAll,

        /// <summary>
        /// Send friend request to target.
        /// Enviar solicitud de amistad al objetivo.
        /// </summary>
        FriendRequest,

        /// <summary>
        /// Block target player.
        /// Bloquear jugador objetivo.
        /// </summary>
        BlockPlayer,

        /// <summary>
        /// Report player behavior.
        /// Reportar comportamiento del jugador.
        /// </summary>
        ReportPlayer,

        /// <summary>
        /// Toggle Streamer Mode (Hide names).
        /// Alternar Modo Streamer (Ocultar nombres).
        /// </summary>
        ToggleStreamerMode,

        /// <summary>
        /// Open Guild/Clan menu.
        /// Abrir menú de Clan/Gremio.
        /// </summary>
        OpenGuildMenu,

        // --- Misc / Minigames ---
        // --- Varios / Minijuegos ---

        /// <summary>
        /// Cast fishing line.
        /// Lanzar caña de pescar.
        /// </summary>
        FishCast,

        /// <summary>
        /// Reel in fishing line.
        /// Recoger sedal de pesca.
        /// </summary>
        FishReel,

        /// <summary>
        /// Play instrument note 1.
        /// Tocar nota de instrumento 1.
        /// </summary>
        PlayNote1,

        /// <summary>
        /// Play instrument note 2.
        /// Tocar nota de instrumento 2.
        /// </summary>
        PlayNote2,

        /// <summary>
        /// Play instrument note 3.
        /// Tocar nota de instrumento 3.
        /// </summary>
        PlayNote3,

        /// <summary>
        /// Place marker on map.
        /// Colocar marcador en el mapa.
        /// </summary>
        MapPlaceMarker,

        /// <summary>
        /// Remove marker from map.
        /// Quitar marcador del mapa.
        /// </summary>
        MapRemoveMarker,

        /// <summary>
        /// Change radio station next.
        /// Siguiente emisora de radio.
        /// </summary>
        RadioNext,

        /// <summary>
        /// Change radio station previous.
        /// Emisora de radio anterior.
        /// </summary>
        RadioPrev,

        /// <summary>
        /// Turn radio on/off.
        /// Encender/Apagar radio.
        /// </summary>
        RadioToggle,
        // --- Movimiento Básico ---
        /// <summary>Acción de moverse hacia arriba.</summary>
        MoverArriba,
        /// <summary>Acción de moverse hacia abajo.</summary>
        MoverAbajo,
        /// <summary>Acción de moverse hacia la izquierda.</summary>
        MoverIzquierda,
        /// <summary>Acción de moverse hacia la derecha.</summary>
        MoverDerecha,
        /// <summary>Acción de saltar.</summary>
        Saltar,
        /// <summary>Acción de realizar un impulso rápido (Dash).</summary>
        Impulso,

        // --- Control de Cámara ---
        /// <summary>Rotar la cámara hacia la izquierda.</summary>
        RotarCamaraIzquierda,
        /// <summary>Rotar la cámara hacia la derecha.</summary>
        RotarCamaraDerecha,
        /// <summary>Rotar la cámara hacia arriba.</summary>
        RotarCamaraArriba,
        /// <summary>Rotar la cámara hacia abajo.</summary>
        RotarCamaraAbajo,
        /// <summary>Acercar la cámara (Zoom In).</summary>
        AcercarCamara,
        /// <summary>Alejar la cámara (Zoom Out).</summary>
        AlejarCamara,
        /// <summary>Reiniciar la cámara a su posición por defecto.</summary>
        ReiniciarCamara,
        /// <summary>Fijar la cámara en el objetivo.</summary>
        FijarObjetivo,
        /// <summary>Alternar entre vista de primera y tercera persona.</summary>
        AlternarPerspectiva,
        /// <summary>Mirar hacia atrás rápidamente.</summary>
        MirarAtras,

        // --- Movimiento Avanzado ---
        /// <summary>Acción de esprintar o correr rápido.</summary>
        Esprintar,
        /// <summary>Alternar entre caminar y correr.</summary>
        AlternarCaminar,
        /// <summary>Acción de agacharse.</summary>
        Agacharse,
        /// <summary>Acción de tumbarse o cuerpo a tierra.</summary>
        Tumbarse,
        /// <summary>Deslizarse por el suelo.</summary>
        Deslizarse,
        /// <summary>Correr por la pared.</summary>
        CorrerPorPared,
        /// <summary>Escalar hacia arriba.</summary>
        EscalarArriba,
        /// <summary>Descender escalando.</summary>
        EscalarAbajo,
        /// <summary>Nadar hacia arriba o salir a la superficie.</summary>
        NadarArriba,
        /// <summary>Bucear o nadar hacia el fondo.</summary>
        NadarAbajo,
        /// <summary>Planear en el aire.</summary>
        Planear,
        /// <summary>Rodar para esquivar.</summary>
        RodarEsquivar,
        /// <summary>Usar el gancho de agarre.</summary>
        UsarGancho,

        // --- Combate y Armas ---
        /// <summary>Disparo o ataque principal.</summary>
        DisparoPrincipal,
        /// <summary>Disparo o ataque secundario.</summary>
        DisparoSecundario,
        /// <summary>Acción de atacar (Genérico).</summary>
        Atacar,
        /// <summary>Recargar el arma.</summary>
        Recargar,
        /// <summary>Realizar un ataque pesado.</summary>
        AtaquePesado,
        /// <summary>Bloquear o defenderse.</summary>
        Bloquear,
        /// <summary>Realizar un desvío (Parry).</summary>
        Desvio,
        /// <summary>Dar una patada.</summary>
        Patear,
        /// <summary>Apuntar con la mira.</summary>
        ApuntarConMira,
        /// <summary>Lanzar una granada.</summary>
        LanzarGranada,
        /// <summary>Cambiar a la siguiente arma.</summary>
        ArmaSiguiente,
        /// <summary>Cambiar al arma anterior.</summary>
        ArmaAnterior,
        /// <summary>Enfundar o desenfundar el arma.</summary>
        EnfundarArma,
        /// <summary>Inspeccionar el arma actual.</summary>
        InspeccionarArma,
        /// <summary>Cambiar modo de disparo.</summary>
        CambiarModoDisparo,
        /// <summary>Equipar arma de la ranura 1.</summary>
        EquiparRanura1,
        /// <summary>Equipar arma de la ranura 2.</summary>
        EquiparRanura2,
        /// <summary>Equipar arma de la ranura 3.</summary>
        EquiparRanura3,
        /// <summary>Equipar arma de la ranura 4.</summary>
        EquiparRanura4,
        /// <summary>Ejecutar un remate o ejecución.</summary>
        Ejecucion,

        // --- Interacción y Objetos ---
        /// <summary>Interacción general.</summary>
        Interactuar,
        /// <summary>Recoger un objeto.</summary>
        Recoger,
        /// <summary>Soltar el objeto actual.</summary>
        SoltarObjeto,
        /// <summary>Usar el objeto seleccionado.</summary>
        UsarObjeto,
        /// <summary>Usar curación.</summary>
        Curarse,
        /// <summary>Restaurar energía o maná.</summary>
        RestaurarEnergia,
        /// <summary>Encender o apagar linterna.</summary>
        Linterna,
        /// <summary>Alternar visión nocturna.</summary>
        VisionNocturna,
        /// <summary>Escanear el entorno.</summary>
        Escanear,

        // --- Interacción Avanzada y Puzles ---
        /// <summary>Examinar detalle.</summary>
        ExaminarDetalle,
        /// <summary>Empujar objeto.</summary>
        EmpujarObjeto,
        /// <summary>Tirar de objeto.</summary>
        TirarObjeto,
        /// <summary>Encender fuego.</summary>
        Encender,
        /// <summary>Apagar fuego.</summary>
        Extinguir,
        /// <summary>Hackear terminal.</summary>
        Hackear,
        /// <summary>Ganzuar cerradura.</summary>
        Ganzuar,
        /// <summary>Combinar objetos.</summary>
        CombinarObjetos,
        /// <summary>Desmontar objeto.</summary>
        Desmontar,
        /// <summary>Rotar puzle horario.</summary>
        PuzleRotarHorario,
        /// <summary>Rotar puzle antihorario.</summary>
        PuzleRotarAntihorario,

        // --- Gestión de Inventario e UI ---
        /// <summary>Mover al alijo.</summary>
        MoverAlAlijo,
        /// <summary>Tomar todo.</summary>
        TomarTodo,
        /// <summary>Dividir pila.</summary>
        DividirPila,
        /// <summary>Marcar objeto.</summary>
        MarcarObjeto,
        /// <summary>Comparar objeto.</summary>
        CompararObjeto,
        /// <summary>Ordenar por nombre.</summary>
        OrdenarPorNombre,
        /// <summary>Ordenar por valor.</summary>
        OrdenarPorValor,
        /// <summary>Leer página siguiente.</summary>
        LeerPaginaSiguiente,
        /// <summary>Leer página anterior.</summary>
        LeerPaginaAnterior,

        // --- Habilidades y Magia ---
        /// <summary>Habilidad 1.</summary>
        Habilidad1,
        /// <summary>Habilidad 2.</summary>
        Habilidad2,
        /// <summary>Habilidad 3.</summary>
        Habilidad3,
        /// <summary>Habilidad 4.</summary>
        Habilidad4,
        /// <summary>Habilidad Definitiva.</summary>
        HabilidadDefinitiva,
        /// <summary>Lanzar hechizo.</summary>
        LanzarHechizo,
        /// <summary>Cancelar lanzamiento.</summary>
        CancelarLanzamiento,
        /// <summary>Invocar montura.</summary>
        InvocarMontura,

        // --- Interfaz de Usuario (UI) ---
        /// <summary>Menú de pausa.</summary>
        MenuPausa,
        /// <summary>Abrir inventario.</summary>
        AbrirInventario,
        /// <summary>Abrir mapa.</summary>
        AbrirMapa,
        /// <summary>Alternar tamaño minimapa.</summary>
        AlternarTamanoMinimapa,
        /// <summary>Abrir diario.</summary>
        AbrirDiario,
        /// <summary>Fijar misión.</summary>
        FijarMision,
        /// <summary>Abrir árbol de habilidades.</summary>
        AbrirArbolHabilidades,
        /// <summary>Abrir ficha de personaje.</summary>
        AbrirFichaPersonaje,
        /// <summary>UI Aceptar.</summary>
        UIAceptar,
        /// <summary>UI Cancelar.</summary>
        UICancelar,
        /// <summary>Guardado rápido.</summary>
        GuardadoRapido,
        /// <summary>Carga rápida.</summary>
        CargaRapida,
        /// <summary>Captura de pantalla.</summary>
        CapturaPantalla,
        /// <summary>Alternar HUD.</summary>
        AlternarHUD,
        /// <summary>Consola de depuración.</summary>
        ConsolaDepuracion,

        // --- Vehículos Terrestres ---
        /// <summary>Acelerar vehículo.</summary>
        VehiculoAcelerar,
        /// <summary>Frenar vehículo.</summary>
        VehiculoFrenar,
        /// <summary>Girar vehículo izquierda.</summary>
        VehiculoGirarIzquierda,
        /// <summary>Girar vehículo derecha.</summary>
        VehiculoGirarDerecha,
        /// <summary>Freno de mano vehículo.</summary>
        VehiculoFrenoMano,
        /// <summary>Turbo vehículo.</summary>
        VehiculoTurbo,
        /// <summary>Bocina vehículo.</summary>
        VehiculoBocina,
        /// <summary>Luces vehículo.</summary>
        VehiculoLuces,
        /// <summary>Salir vehículo.</summary>
        VehiculoSalir,

        // --- Vuelo y Espacio (6DOF) ---
        /// <summary>Guiñada izquierda (Vuelo).</summary>
        VueloGuinadaIzquierda,
        /// <summary>Guiñada derecha (Vuelo).</summary>
        VueloGuinadaDerecha,
        /// <summary>Alabeo izquierda (Vuelo).</summary>
        VueloAlabeoIzquierda,
        /// <summary>Alabeo derecha (Vuelo).</summary>
        VueloAlabeoDerecha,
        /// <summary>Cabeceo arriba (Vuelo).</summary>
        VueloCabeceoArriba,
        /// <summary>Cabeceo abajo (Vuelo).</summary>
        VueloCabeceoAbajo,
        /// <summary>Empuje vertical arriba.</summary>
        EmpujeVerticalArriba,
        /// <summary>Empuje vertical abajo.</summary>
        EmpujeVerticalAbajo,
        /// <summary>Empuje lateral izquierda.</summary>
        EmpujeLateralIzquierda,
        /// <summary>Empuje lateral derecha.</summary>
        EmpujeLateralDerecha,
        /// <summary>Alternar tren de aterrizaje.</summary>
        TrenAterrizaje,
        /// <summary>Activar hipervelocidad.</summary>
        ActivarHipervelocidad,
        /// <summary>Desplegar contramedidas.</summary>
        DesplegarContramedidas,
        /// <summary>Objetivo enemigo más cercano.</summary>
        ObjetivoEnemigoCercano,
        /// <summary>Ciclar objetivos.</summary>
        CiclarObjetivos,
        /// <summary>Ciclar subsistemas.</summary>
        CiclarSubsistemas,

        // --- Multijugador y Social ---
        /// <summary>Pulsar para hablar.</summary>
        PulsarParaHablar,
        /// <summary>Abrir chat.</summary>
        AbrirChat,
        /// <summary>Mostrar puntuaciones.</summary>
        MostrarPuntuaciones,
        /// <summary>Marcar ubicación (Ping).</summary>
        MarcarUbicacion,
        /// <summary>Marcar enemigo.</summary>
        MarcarEnemigo,
        /// <summary>Emote: Saludar.</summary>
        GestoSaludar,
        /// <summary>Emote: Bailar.</summary>
        GestoBailar,
        /// <summary>Emote: Burlarse.</summary>
        GestoBurlarse,
        /// <summary>Votar Sí.</summary>
        VotarSi,
        /// <summary>Votar No.</summary>
        VotarNo,
        /// <summary>Silenciar todos.</summary>
        SilenciarTodos,
        /// <summary>Reactivar audio todos.</summary>
        ReactivarAudioTodos,
        /// <summary>Solicitud de amistad.</summary>
        SolicitudAmistad,
        /// <summary>Bloquear jugador.</summary>
        BloquearJugador,
        /// <summary>Reportar jugador.</summary>
        ReportarJugador,
        /// <summary>Modo Streamer.</summary>
        ModoStreamer,
        /// <summary>Menú de clan.</summary>
        MenuClan,

        // --- Comandos de Escuadrón ---
        /// <summary>Escuadrón: Seguir.</summary>
        EscuadronSeguir,
        /// <summary>Escuadrón: Alto el fuego.</summary>
        EscuadronAltoElFuego,
        /// <summary>Escuadrón: Fuego a discreción.</summary>
        EscuadronFuegoLibre,
        /// <summary>Escuadrón: Ir a posición.</summary>
        EscuadronIrA,
        /// <summary>Escuadrón: Reagruparse.</summary>
        EscuadronReagruparse,
        /// <summary>Escuadrón: Flanquear izquierda.</summary>
        EscuadronFlanquearIzq,
        /// <summary>Escuadrón: Flanquear derecha.</summary>
        EscuadronFlanquearDer,
        /// <summary>Escuadrón: Cubrirse.</summary>
        EscuadronCubrirse,
        /// <summary>Compañero: Habilidad especial.</summary>
        CompaneroHabilidadEspecial,
        /// <summary>Cambiar personaje siguiente.</summary>
        CambiarPersonajeSiguiente,
        /// <summary>Cambiar personaje anterior.</summary>
        CambiarPersonajeAnterior,

        // --- Estrategia y Construcción ---
        /// <summary>Rotar objeto izquierda.</summary>
        RotarObjetoIzquierda,
        /// <summary>Rotar objeto derecha.</summary>
        RotarObjetoDerecha,
        /// <summary>Confirmar construcción.</summary>
        ConfirmarConstruccion,
        /// <summary>Cancelar construcción.</summary>
        CancelarConstruccion,
        /// <summary>Seleccionar todas las unidades.</summary>
        SeleccionarTodo,
        /// <summary>Comando: Mover.</summary>
        ComandoMover,
        /// <summary>Comando: Atacar.</summary>
        ComandoAtacar,
        /// <summary>Comando: Mantener posición.</summary>
        ComandoMantener,

        // --- Modo Editor / Creativo ---
        /// <summary>Alternar Modo Foto.</summary>
        ModoFoto,
        /// <summary>Aumentar campo de visión.</summary>
        AumentarFOV,
        /// <summary>Disminuir campo de visión.</summary>
        DisminuirFOV,
        /// <summary>Aumentar profundidad de campo.</summary>
        AumentarDOF,
        /// <summary>Disminuir profundidad de campo.</summary>
        DisminuirDOF,
        /// <summary>Alternar ajuste a rejilla.</summary>
        AjusteRejilla,
        /// <summary>Duplicar objeto.</summary>
        DuplicarObjeto,
        /// <summary>Eliminar objeto.</summary>
        EliminarObjeto,
        /// <summary>Deshacer acción.</summary>
        Deshacer,
        /// <summary>Rehacer acción.</summary>
        Rehacer,
        /// <summary>Guardar plano.</summary>
        GuardarPlano,
        /// <summary>Cargar plano.</summary>
        CargarPlano,

        // --- Realidad Virtual (VR) ---
        /// <summary>Recentrar VR.</summary>
        VRRecentrar,
        /// <summary>Teletransporte VR.</summary>
        VRTeletransporte,
        /// <summary>Agarre mano izquierda.</summary>
        VRAgarreIzquierda,
        /// <summary>Agarre mano derecha.</summary>
        VRAgarreDerecha,
        /// <summary>Gatillo mano izquierda.</summary>
        VRGatilloIzquierda,
        /// <summary>Gatillo mano derecha.</summary>
        VRGatilloDerecha,
        /// <summary>Giro rápido izquierda.</summary>
        VRGiroRapidoIzquierda,
        /// <summary>Giro rápido derecha.</summary>
        VRGiroRapidoDerecha,

        // --- Depuración y Desarrollador ---
        /// <summary>Modo Dios.</summary>
        ModoDios,
        /// <summary>Modo NoClip (Atravesar).</summary>
        ModoNoClip,
        /// <summary>Ralentizar tiempo.</summary>
        TiempoLento,
        /// <summary>Acelerar tiempo.</summary>
        TiempoRapido,

        // --- Varios ---
        /// <summary>Lanzar caña de pescar.</summary>
        PescaLanzar,
        /// <summary>Recoger caña de pescar.</summary>
        PescaRecoger,
        /// <summary>Tocar nota 1.</summary>
        TocarNota1,
        /// <summary>Tocar nota 2.</summary>
        TocarNota2,
        /// <summary>Tocar nota 3.</summary>
        TocarNota3,
        /// <summary>Poner marcador mapa.</summary>
        MapaPonerMarcador,
        /// <summary>Quitar marcador mapa.</summary>
        MapaQuitarMarcador,
        /// <summary>Radio siguiente.</summary>
        RadioSiguiente,
        /// <summary>Radio anterior.</summary>
        RadioAnterior,
        /// <summary>Radio encender/apagar.</summary>
        RadioInterruptor
    }
}
