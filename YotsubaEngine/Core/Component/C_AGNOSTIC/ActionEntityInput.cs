using System;
using System.Collections.Generic;
using System.Text;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    /// <summary>
    /// Acciones de la entidad, para mapear con uno o varios inputs
    /// <para>Entity actions mapped to input bindings.</para>
    /// </summary>
    public enum ActionEntityInput
    {
        /// <summary>
        /// Acción de moverse hacia arriba.
        /// <para>Move up action.</para>
        /// </summary>
        MoveUp,
        /// <summary>
        /// Acción de moverse hacia abajo.
        /// <para>Move down action.</para>
        /// </summary>
        MoveDown,
        /// <summary>
        /// Acción de moverse hacia la izquierda.
        /// <para>Move left action.</para>
        /// </summary>
        MoveLeft,
        /// <summary>
        /// Acción de moverse hacia la derecha.
        /// <para>Move right action.</para>
        /// </summary>
        MoveRight,
        /// <summary>
        /// Acción de atacar.
        /// <para>Attack action.</para>
        /// </summary>
        Attack,
        /// <summary>
        /// Acción de saltar.
        /// <para>Jump action.</para>
        /// </summary>
        Jump,
        /// <summary>
        /// Acción de dash.
        /// <para>Dash action.</para>
        /// </summary>
        Dash,

        // --- Camera Actions (Completed from your list) ---
        // --- Acciones de Cámara (Completando tu lista) ---

        /// <summary>
        /// Acción de rotar la cámara a la izquierda.
        /// <para>Rotate camera left action.</para>
        /// </summary>
        RotateCameraLeft,

        /// <summary>
        /// Acción de rotar la cámara a la derecha.
        /// <para>Rotate camera right action.</para>
        /// </summary>
        RotateCameraRight,

        /// <summary>
        /// Acción de rotar la cámara hacia arriba.
        /// <para>Rotate camera up action.</para>
        /// </summary>
        RotateCameraUp,

        /// <summary>
        /// Acción de rotar la cámara hacia abajo.
        /// <para>Rotate camera down action.</para>
        /// </summary>
        RotateCameraDown,

        /// <summary>
        /// Acción de acercar la cámara (Zoom in).
        /// <para>Zoom camera in.</para>
        /// </summary>
        CameraZoomIn,

        /// <summary>
        /// Acción de alejar la cámara (Zoom out).
        /// <para>Zoom camera out.</para>
        /// </summary>
        CameraZoomOut,

        /// <summary>
        /// Acción de reiniciar la cámara a su posición por defecto.
        /// <para>Reset camera to default position.</para>
        /// </summary>
        CameraReset,

        /// <summary>
        /// Acción de fijar la cámara en el objetivo.
        /// <para>Lock camera on target.</para>
        /// </summary>
        CameraLockOn,

        /// <summary>
        /// Alternar entre vista de primera y tercera persona.
        /// <para>Toggle between first and third person view.</para>
        /// </summary>
        TogglePerspective,

        /// <summary>
        /// Acción de mirar hacia atrás.
        /// <para>Look behind action.</para>
        /// </summary>
        LookBehind,

        // --- Advanced Movement ---
        // --- Movimiento Avanzado ---

        /// <summary>
        /// Acción de correr o esprintar.
        /// <para>Sprint action.</para>
        /// </summary>
        Sprint,

        /// <summary>
        /// Acción de alternar modo caminar/correr.
        /// <para>Toggle walking mode.</para>
        /// </summary>
        WalkToggle,

        /// <summary>
        /// Acción de agacharse.
        /// <para>Crouch action.</para>
        /// </summary>
        Crouch,

        /// <summary>
        /// Acción de tumbarse o cuerpo a tierra.
        /// <para>Prone action (lie down).</para>
        /// </summary>
        Prone,

        /// <summary>
        /// Acción de deslizarse por el suelo.
        /// <para>Slide action.</para>
        /// </summary>
        Slide,

        /// <summary>
        /// Acción de correr por la pared.
        /// <para>Wall run action.</para>
        /// </summary>
        WallRun,

        /// <summary>
        /// Acción de escalar hacia arriba.
        /// <para>Climb up action.</para>
        /// </summary>
        ClimbUp,

        /// <summary>
        /// Acción de descender escalando.
        /// <para>Climb down action.</para>
        /// </summary>
        ClimbDown,

        /// <summary>
        /// Acción de nadar hacia arriba o salir a la superficie.
        /// <para>Swim up / Surface.</para>
        /// </summary>
        SwimUp,

        /// <summary>
        /// Acción de bucear o nadar hacia abajo.
        /// <para>Dive / Swim down.</para>
        /// </summary>
        SwimDown,

        /// <summary>
        /// Acción de planear en el aire.
        /// <para>Glide action.</para>
        /// </summary>
        Glide,

        /// <summary>
        /// Acción de rodar para esquivar.
        /// <para>Dodge roll action.</para>
        /// </summary>
        DodgeRoll,

        /// <summary>
        /// Acción de usar el gancho de agarre.
        /// <para>Grappling hook action.</para>
        /// </summary>
        Grapple,

        // --- Combat & Weapons ---
        // --- Combate y Armas ---

        /// <summary>
        /// Disparo principal.
        /// <para>Primary fire action.</para>
        /// </summary>
        PrimaryFire,

        /// <summary>
        /// Disparo secundario.
        /// <para>Secondary fire action.</para>
        /// </summary>
        SecondaryFire,

        /// <summary>
        /// Recargar arma.
        /// <para>Reload weapon.</para>
        /// </summary>
        Reload,

        /// <summary>
        /// Ataque pesado.
        /// <para>Heavy attack action.</para>
        /// </summary>
        HeavyAttack,

        /// <summary>
        /// Acción de bloquear o defender.
        /// <para>Block or defend action.</para>
        /// </summary>
        Block,

        /// <summary>
        /// Acción de parry (desvío).
        /// <para>Parry action.</para>
        /// </summary>
        Parry,

        /// <summary>
        /// Acción de patear.
        /// <para>Kick action.</para>
        /// </summary>
        Kick,

        /// <summary>
        /// Acción de apuntar con la mira.
        /// <para>Aim down sights (ADS).</para>
        /// </summary>
        AimDownSights,

        /// <summary>
        /// Lanzar granada.
        /// <para>Throw grenade.</para>
        /// </summary>
        ThrowGrenade,

        /// <summary>
        /// Cambiar a la siguiente arma.
        /// <para>Switch to next weapon.</para>
        /// </summary>
        NextWeapon,

        /// <summary>
        /// Cambiar al arma anterior.
        /// <para>Switch to previous weapon.</para>
        /// </summary>
        PreviousWeapon,

        /// <summary>
        /// Enfundar o desenfundar arma.
        /// <para>Holster or unholster weapon.</para>
        /// </summary>
        HolsterWeapon,

        /// <summary>
        /// Inspeccionar el arma actual.
        /// <para>Inspect current weapon.</para>
        /// </summary>
        InspectWeapon,

        /// <summary>
        /// Cambiar modo de disparo (Único, Ráfaga, Auto).
        /// <para>Change firing mode (Single, Burst, Auto).</para>
        /// </summary>
        ChangeFireMode,

        /// <summary>
        /// Equipar arma de la ranura 1.
        /// <para>Equip weapon in slot 1.</para>
        /// </summary>
        EquipSlot1,

        /// <summary>
        /// Equipar arma de la ranura 2.
        /// <para>Equip weapon in slot 2.</para>
        /// </summary>
        EquipSlot2,

        /// <summary>
        /// Equipar arma de la ranura 3.
        /// <para>Equip weapon in slot 3.</para>
        /// </summary>
        EquipSlot3,

        /// <summary>
        /// Equipar arma de la ranura 4.
        /// <para>Equip weapon in slot 4.</para>
        /// </summary>
        EquipSlot4,

        /// <summary>
        /// Usar ejecución cuerpo a cuerpo / remate.
        /// <para>Use melee execution / finisher.</para>
        /// </summary>
        Execution,

        // --- Interaction & Items ---
        // --- Interacción y Objetos ---

        /// <summary>
        /// Interacción general (Hablar, Abrir, etc.).
        /// <para>General interaction (Talk, Open, etc.).</para>
        /// </summary>
        Interact,

        /// <summary>
        /// Recoger objeto.
        /// <para>Pick up item.</para>
        /// </summary>
        PickUp,

        /// <summary>
        /// Soltar objeto actual.
        /// <para>Drop current item.</para>
        /// </summary>
        DropItem,

        /// <summary>
        /// Usar el objeto seleccionado actualmente.
        /// <para>Use currently selected item.</para>
        /// </summary>
        UseItem,

        /// <summary>
        /// Usar poción de salud / botiquín.
        /// <para>Use health potion / medkit.</para>
        /// </summary>
        Heal,

        /// <summary>
        /// Usar poción de maná / paquete de energía.
        /// <para>Use mana potion / energy pack.</para>
        /// </summary>
        RestoreEnergy,

        /// <summary>
        /// Alternar linterna.
        /// <para>Toggle flashlight.</para>
        /// </summary>
        Flashlight,

        /// <summary>
        /// Alternar visión nocturna.
        /// <para>Toggle night vision.</para>
        /// </summary>
        NightVision,

        /// <summary>
        /// Escanear el entorno.
        /// <para>Scan environment.</para>
        /// </summary>
        Scan,

        // --- Skills & Magic ---
        // --- Habilidades y Magia ---

        /// <summary>
        /// Activar Habilidad 1.
        /// <para>Activate Skill 1.</para>
        /// </summary>
        Skill1,

        /// <summary>
        /// Activar Habilidad 2.
        /// <para>Activate Skill 2.</para>
        /// </summary>
        Skill2,

        /// <summary>
        /// Activar Habilidad 3.
        /// <para>Activate Skill 3.</para>
        /// </summary>
        Skill3,

        /// <summary>
        /// Activar Habilidad 4.
        /// <para>Activate Skill 4.</para>
        /// </summary>
        Skill4,

        /// <summary>
        /// Activar Habilidad Definitiva (Ultimate).
        /// <para>Activate Ultimate Ability.</para>
        /// </summary>
        UltimateAbility,

        /// <summary>
        /// Lanzar hechizo seleccionado.
        /// <para>Cast selected spell.</para>
        /// </summary>
        CastSpell,

        /// <summary>
        /// Cancelar lanzamiento actual.
        /// <para>Cancel current casting.</para>
        /// </summary>
        CancelCast,

        /// <summary>
        /// Invocar montura o vehículo.
        /// <para>Summon mount or vehicle.</para>
        /// </summary>
        SummonMount,

        // --- UI & System ---
        // --- Interfaz y Sistema ---

        /// <summary>
        /// Alternar Menú de Pausa.
        /// <para>Toggle Pause Menu.</para>
        /// </summary>
        PauseMenu,

        /// <summary>
        /// Abrir Inventario.
        /// <para>Open Inventory.</para>
        /// </summary>
        OpenInventory,

        /// <summary>
        /// Abrir Mapa.
        /// <para>Open Map.</para>
        /// </summary>
        OpenMap,

        /// <summary>
        /// Abrir Diario de Misiones.
        /// <para>Open Quest Log / Journal.</para>
        /// </summary>
        OpenJournal,

        /// <summary>
        /// Abrir Árbol de Habilidades.
        /// <para>Open Skill Tree.</para>
        /// </summary>
        OpenSkillTree,

        /// <summary>
        /// Abrir Ficha de Personaje / Estadísticas.
        /// <para>Open Character Sheet / Stats.</para>
        /// </summary>
        OpenCharacterSheet,

        /// <summary>
        /// UI Aceptar / Confirmar.
        /// <para>UI Submit / Accept.</para>
        /// </summary>
        UISubmit,

        /// <summary>
        /// UI Cancelar / Atrás.
        /// <para>UI Cancel / Back.</para>
        /// </summary>
        UICancel,

        /// <summary>
        /// Guardado Rápido.
        /// <para>Quick Save game.</para>
        /// </summary>
        QuickSave,

        /// <summary>
        /// Carga Rápida.
        /// <para>Quick Load game.</para>
        /// </summary>
        QuickLoad,

        /// <summary>
        /// Tomar captura de pantalla.
        /// <para>Take screenshot.</para>
        /// </summary>
        Screenshot,

        /// <summary>
        /// Alternar visibilidad del HUD.
        /// <para>Toggle HUD visibility.</para>
        /// </summary>
        ToggleHUD,

        // --- Vehicle Controls ---
        // --- Controles de Vehículos ---

        /// <summary>
        /// Acelerar vehículo.
        /// <para>Vehicle Accelerate / Throttle.</para>
        /// </summary>
        VehicleAccelerate,

        /// <summary>
        /// Frenar vehículo / Marcha atrás.
        /// <para>Vehicle Brake / Reverse.</para>
        /// </summary>
        VehicleBrake,

        /// <summary>
        /// Girar vehículo a la izquierda.
        /// <para>Vehicle Turn Left.</para>
        /// </summary>
        VehicleTurnLeft,

        /// <summary>
        /// Girar vehículo a la derecha.
        /// <para>Vehicle Turn Right.</para>
        /// </summary>
        VehicleTurnRight,

        /// <summary>
        /// Freno de mano / Derrape.
        /// <para>Vehicle Handbrake / Drift.</para>
        /// </summary>
        VehicleHandbrake,

        /// <summary>
        /// Turbo / Nitro del vehículo.
        /// <para>Vehicle Boost / Nitro.</para>
        /// </summary>
        VehicleBoost,

        /// <summary>
        /// Usar bocina (claxon).
        /// <para>Use Vehicle Horn.</para>
        /// </summary>
        VehicleHorn,

        /// <summary>
        /// Alternar luces del vehículo.
        /// <para>Toggle Vehicle Lights.</para>
        /// </summary>
        VehicleLights,

        /// <summary>
        /// Salir del vehículo.
        /// <para>Exit Vehicle.</para>
        /// </summary>
        VehicleExit,

        /// <summary>
        /// Inclinar morro arriba (Aire/Agua).
        /// <para>Pitch vehicle nose up (Air/Water).</para>
        /// </summary>
        VehiclePitchUp,

        /// <summary>
        /// Inclinar morro abajo (Aire/Agua).
        /// <para>Pitch vehicle nose down (Air/Water).</para>
        /// </summary>
        VehiclePitchDown,

        // --- Multiplayer & Social ---
        // --- Multijugador y Social ---

        /// <summary>
        /// Pulsar para hablar.
        /// <para>Push to talk.</para>
        /// </summary>
        PushToTalk,

        /// <summary>
        /// Abrir chat de texto.
        /// <para>Open Text Chat.</para>
        /// </summary>
        OpenChat,

        /// <summary>
        /// Mostrar tabla de puntuaciones.
        /// <para>Show Scoreboard.</para>
        /// </summary>
        ShowScoreboard,

        /// <summary>
        /// Marcar ubicación contextualmente (Ping).
        /// <para>Mark/Ping location contextually.</para>
        /// </summary>
        PingLocation,

        /// <summary>
        /// Marcar enemigo.
        /// <para>Mark/Ping enemy.</para>
        /// </summary>
        PingEnemy,

        /// <summary>
        /// Emote: Saludar.
        /// <para>Emote: Wave.</para>
        /// </summary>
        EmoteWave,

        /// <summary>
        /// Emote: Bailar.
        /// <para>Emote: Dance.</para>
        /// </summary>
        EmoteDance,

        /// <summary>
        /// Emote: Burlarse.
        /// <para>Emote: Taunt.</para>
        /// </summary>
        EmoteTaunt,

        /// <summary>
        /// Votar Sí / Estar de acuerdo.
        /// <para>Vote Yes / Agree.</para>
        /// </summary>
        VoteYes,

        /// <summary>
        /// Votar No / No estar de acuerdo.
        /// <para>Vote No / Disagree.</para>
        /// </summary>
        VoteNo,

        // --- Building & Strategy (RTS/Builder Context) ---
        // --- Construcción y Estrategia ---

        /// <summary>
        /// Rotar objeto a colocar a la izquierda.
        /// <para>Rotate placement object left.</para>
        /// </summary>
        RotateObjectLeft,

        /// <summary>
        /// Rotar objeto a colocar a la derecha.
        /// <para>Rotate placement object right.</para>
        /// </summary>
        RotateObjectRight,

        /// <summary>
        /// Confirmar colocación / construir.
        /// <para>Confirm placement / build.</para>
        /// </summary>
        BuildConfirm,

        /// <summary>
        /// Cancelar colocación.
        /// <para>Cancel placement.</para>
        /// </summary>
        BuildCancel,

        /// <summary>
        /// Seleccionar todas las unidades.
        /// <para>Select all units.</para>
        /// </summary>
        SelectAllUnits,

        /// <summary>
        /// Orden: Mover a.
        /// <para>Command: Move to.</para>
        /// </summary>
        CommandMove,

        /// <summary>
        /// Orden: Atacar objetivo.
        /// <para>Command: Attack target.</para>
        /// </summary>
        CommandAttack,

        /// <summary>
        /// Orden: Mantener posición.
        /// <para>Command: Hold position.</para>
        /// </summary>
        CommandHold,

        // --- Debug & Developer ---
        // --- Depuración y Desarrollador ---

        /// <summary>
        /// Alternar consola de depuración.
        /// <para>Toggle Debug Console.</para>
        /// </summary>
        ToggleConsole,

        /// <summary>
        /// Alternar Modo Dios.
        /// <para>Toggle God Mode.</para>
        /// </summary>
        ToggleGodMode,

        /// <summary>
        /// Alternar modo NoClip (atravesar paredes).
        /// <para>Toggle NoClip mode.</para>
        /// </summary>
        ToggleNoClip,

        /// <summary>
        /// Ralentizar tiempo de juego.
        /// <para>Slow down game time.</para>
        /// </summary>
        TimeSlow,

        /// <summary>
        /// Acelerar tiempo de juego.
        /// <para>Speed up game time.</para>
        /// </summary>
        TimeSpeedUp,
        // --- Advanced Interaction / Puzzle ---
        // --- Interacción Avanzada / Puzles ---

        /// <summary>
        /// Examinar vista detallada de un objeto.
        /// <para>Examine detailed view of an object.</para>
        /// </summary>
        ExamineDetail,

        /// <summary>
        /// Empujar objeto hacia adelante.
        /// <para>Push object forward.</para>
        /// </summary>
        PushObject,

        /// <summary>
        /// Tirar del objeto hacia atrás.
        /// <para>Pull object backward.</para>
        /// </summary>
        PullObject,

        /// <summary>
        /// Encender antorcha o fuente de luz.
        /// <para>Ignite torch or light source.</para>
        /// </summary>
        Ignite,

        /// <summary>
        /// Apagar fuego o fuente de luz.
        /// <para>Extinguish fire or light source.</para>
        /// </summary>
        Extinguish,

        /// <summary>
        /// Hackear terminal o computadora.
        /// <para>Hack terminal or computer.</para>
        /// </summary>
        Hack,

        /// <summary>
        /// Ganzuar cerradura.
        /// <para>Pick lock.</para>
        /// </summary>
        PickLock,

        /// <summary>
        /// Combinar objetos en el menú de crafteo.
        /// <para>Combine items in crafting menu.</para>
        /// </summary>
        CombineItems,

        /// <summary>
        /// Desmontar objeto para piezas.
        /// <para>Disassemble item for parts.</para>
        /// </summary>
        Disassemble,

        /// <summary>
        /// Rotar pieza de puzle en sentido horario.
        /// <para>Rotate puzzle piece clockwise.</para>
        /// </summary>
        PuzzleRotateCW,

        /// <summary>
        /// Rotar pieza de puzle en sentido antihorario.
        /// <para>Rotate puzzle piece counter-clockwise.</para>
        /// </summary>
        PuzzleRotateCCW,

        // --- Advanced Inventory & UI Management ---
        // --- Gestión Avanzada de Inventario e UI ---

        /// <summary>
        /// Mover objeto al alijo/cofre.
        /// <para>Move item to stash/chest.</para>
        /// </summary>
        MoveToStash,

        /// <summary>
        /// Tomar todos los objetos del contenedor.
        /// <para>Take all items from container.</para>
        /// </summary>
        TakeAll,

        /// <summary>
        /// Dividir pila de objetos.
        /// <para>Split item stack.</para>
        /// </summary>
        SplitStack,

        /// <summary>
        /// Marcar objeto como basura/favorito.
        /// <para>Mark item as junk/favorite.</para>
        /// </summary>
        MarkItem,

        /// <summary>
        /// Comparar objeto con el equipado.
        /// <para>Compare item with equipped.</para>
        /// </summary>
        CompareItem,

        /// <summary>
        /// Ordenar inventario por nombre.
        /// <para>Sort inventory by name.</para>
        /// </summary>
        SortByName,

        /// <summary>
        /// Ordenar inventario por peso/valor.
        /// <para>Sort inventory by weight/value.</para>
        /// </summary>
        SortByValue,

        /// <summary>
        /// Alternar tamaño del mini-mapa.
        /// <para>Toggle mini-map size (small/large).</para>
        /// </summary>
        ToggleMiniMapSize,

        /// <summary>
        /// Fijar misión en el HUD.
        /// <para>Pin quest to HUD.</para>
        /// </summary>
        PinQuest,

        /// <summary>
        /// Leer página siguiente.
        /// <para>Read next page (book/note).</para>
        /// </summary>
        ReadNextPage,

        /// <summary>
        /// Leer página anterior.
        /// <para>Read previous page (book/note).</para>
        /// </summary>
        ReadPrevPage,

        // --- Flight & Space Sim (6DOF) ---
        // --- Simulación de Vuelo y Espacio (6DOF) ---

        /// <summary>
        /// Guiñada a la izquierda (Vuelo).
        /// <para>Yaw left (Flight).</para>
        /// </summary>
        FlightYawLeft,

        /// <summary>
        /// Guiñada a la derecha (Vuelo).
        /// <para>Yaw right (Flight).</para>
        /// </summary>
        FlightYawRight,

        /// <summary>
        /// Alabeo a la izquierda (Vuelo).
        /// <para>Roll left (Flight).</para>
        /// </summary>
        FlightRollLeft,

        /// <summary>
        /// Alabeo a la derecha (Vuelo).
        /// <para>Roll right (Flight).</para>
        /// </summary>
        FlightRollRight,

        /// <summary>
        /// Cabeceo arriba (Vuelo).
        /// <para>Pitch up (Flight).</para>
        /// </summary>
        FlightPitchUp,

        /// <summary>
        /// Cabeceo abajo (Vuelo).
        /// <para>Pitch down (Flight).</para>
        /// </summary>
        FlightPitchDown,

        /// <summary>
        /// Empuje vertical arriba.
        /// <para>Vertical thrust up (VTOL/Space).</para>
        /// </summary>
        ThrustUp,

        /// <summary>
        /// Empuje vertical abajo.
        /// <para>Vertical thrust down (VTOL/Space).</para>
        /// </summary>
        ThrustDown,

        /// <summary>
        /// Empuje lateral izquierdo.
        /// <para>Lateral thrust left (Space).</para>
        /// </summary>
        ThrustLeft,

        /// <summary>
        /// Empuje lateral derecho.
        /// <para>Lateral thrust right (Space).</para>
        /// </summary>
        ThrustRight,

        /// <summary>
        /// Alternar tren de aterrizaje.
        /// <para>Toggle landing gear.</para>
        /// </summary>
        ToggleLandingGear,

        /// <summary>
        /// Activar motor de curvatura / hiperimpulsor.
        /// <para>Engage warp drive / hyperdrive.</para>
        /// </summary>
        EngageWarp,

        /// <summary>
        /// Desplegar contramedidas (bengalas).
        /// <para>Deploy countermeasures (flares/chaff).</para>
        /// </summary>
        DeployCountermeasures,

        /// <summary>
        /// Fijar enemigo más cercano.
        /// <para>Target nearest enemy.</para>
        /// </summary>
        TargetNearestEnemy,

        /// <summary>
        /// Ciclar objetivos hostiles.
        /// <para>Cycle hostile targets.</para>
        /// </summary>
        CycleTargets,

        /// <summary>
        /// Ciclar subsistemas del objetivo (motores, armas).
        /// <para>Cycle sub-systems targeting (engines, weapons).</para>
        /// </summary>
        CycleSubsystems,

        // --- Squad & Companion Command ---
        // --- Comandos de Escuadrón y Compañeros ---

        /// <summary>
        /// Orden: Síganme.
        /// <para>Command squad: Follow me.</para>
        /// </summary>
        SquadFollow,

        /// <summary>
        /// Orden: Alto el fuego.
        /// <para>Command squad: Hold fire.</para>
        /// </summary>
        SquadHoldFire,

        /// <summary>
        /// Orden: Fuego a discreción.
        /// <para>Command squad: Open fire.</para>
        /// </summary>
        SquadOpenFire,

        /// <summary>
        /// Orden: Ir al punto.
        /// <para>Command squad: Go to point.</para>
        /// </summary>
        SquadGoTo,

        /// <summary>
        /// Orden: Reagruparse.
        /// <para>Command squad: Regroup.</para>
        /// </summary>
        SquadRegroup,

        /// <summary>
        /// Orden: Flanquear izquierda.
        /// <para>Command squad: Flank left.</para>
        /// </summary>
        SquadFlankLeft,

        /// <summary>
        /// Orden: Flanquear derecha.
        /// <para>Command squad: Flank right.</para>
        /// </summary>
        SquadFlankRight,

        /// <summary>
        /// Orden: Cubrirse.
        /// <para>Command squad: Take cover.</para>
        /// </summary>
        SquadCover,

        /// <summary>
        /// Orden compañero: Usar habilidad especial.
        /// <para>Command companion: Use special ability.</para>
        /// </summary>
        CompanionSpecial,

        /// <summary>
        /// Cambiar control al siguiente personaje.
        /// <para>Swap control to next character.</para>
        /// </summary>
        SwapCharacterNext,

        /// <summary>
        /// Cambiar control al personaje anterior.
        /// <para>Swap control to previous character.</para>
        /// </summary>
        SwapCharacterPrev,

        // --- Editor / Photo Mode / Creative ---
        // --- Modo Editor / Foto / Creativo ---

        /// <summary>
        /// Alternar Modo Foto.
        /// <para>Toggle Photo Mode.</para>
        /// </summary>
        TogglePhotoMode,

        /// <summary>
        /// Aumentar campo de visión (FOV).
        /// <para>Increase Field of View (FOV).</para>
        /// </summary>
        IncreaseFOV,

        /// <summary>
        /// Disminuir campo de visión (FOV).
        /// <para>Decrease Field of View (FOV).</para>
        /// </summary>
        DecreaseFOV,

        /// <summary>
        /// Aumentar profundidad de campo.
        /// <para>Increase Depth of Field (DOF).</para>
        /// </summary>
        IncreaseDOF,

        /// <summary>
        /// Disminuir profundidad de campo.
        /// <para>Decrease Depth of Field (DOF).</para>
        /// </summary>
        DecreaseDOF,

        /// <summary>
        /// Alternar ajuste a la rejilla.
        /// <para>Toggle Grid Snapping.</para>
        /// </summary>
        ToggleGridSnap,

        /// <summary>
        /// Duplicar objeto seleccionado.
        /// <para>Duplicate selected object.</para>
        /// </summary>
        DuplicateObject,

        /// <summary>
        /// Borrar objeto seleccionado.
        /// <para>Delete selected object.</para>
        /// </summary>
        DeleteObject,

        /// <summary>
        /// Deshacer última acción.
        /// <para>Undo last action (Editor).</para>
        /// </summary>
        UndoAction,

        /// <summary>
        /// Rehacer última acción.
        /// <para>Redo last action (Editor).</para>
        /// </summary>
        RedoAction,

        /// <summary>
        /// Guardar esquema/plano.
        /// <para>Save schematic/blueprint.</para>
        /// </summary>
        SaveBlueprint,

        /// <summary>
        /// Cargar esquema/plano.
        /// <para>Load schematic/blueprint.</para>
        /// </summary>
        LoadBlueprint,

        // --- VR (Virtual Reality) Specific ---
        // --- Específicas de VR ---

        /// <summary>
        /// Recentrar vista VR.
        /// <para>Recenter VR View.</para>
        /// </summary>
        VRRecenter,

        /// <summary>
        /// Teletransportarse (Locomoción VR).
        /// <para>Teleport (VR Locomotion).</para>
        /// </summary>
        VRTeleport,

        /// <summary>
        /// Agarrar con mano izquierda.
        /// <para>Grip Left Hand.</para>
        /// </summary>
        VRGripLeft,

        /// <summary>
        /// Agarrar con mano derecha.
        /// <para>Grip Right Hand.</para>
        /// </summary>
        VRGripRight,

        /// <summary>
        /// Gatillo mano izquierda.
        /// <para>Trigger Left Hand.</para>
        /// </summary>
        VRTriggerLeft,

        /// <summary>
        /// Gatillo mano derecha.
        /// <para>Trigger Right Hand.</para>
        /// </summary>
        VRTriggerRight,

        /// <summary>
        /// Giro rápido izquierda (VR).
        /// <para>Snap Turn Left (VR).</para>
        /// </summary>
        VRSnapTurnLeft,

        /// <summary>
        /// Giro rápido derecha (VR).
        /// <para>Snap Turn Right (VR).</para>
        /// </summary>
        VRSnapTurnRight,

        // --- Social / Chat / Emotes ---
        // --- Social / Chat / Emoticonos ---

        /// <summary>
        /// Silenciar a todos los jugadores.
        /// <para>Mute all players.</para>
        /// </summary>
        MuteAll,

        /// <summary>
        /// Reactivar audio de todos.
        /// <para>Unmute all players.</para>
        /// </summary>
        UnmuteAll,

        /// <summary>
        /// Enviar solicitud de amistad al objetivo.
        /// <para>Send friend request to target.</para>
        /// </summary>
        FriendRequest,

        /// <summary>
        /// Bloquear jugador objetivo.
        /// <para>Block target player.</para>
        /// </summary>
        BlockPlayer,

        /// <summary>
        /// Reportar comportamiento del jugador.
        /// <para>Report player behavior.</para>
        /// </summary>
        ReportPlayer,

        /// <summary>
        /// Alternar Modo Streamer (Ocultar nombres).
        /// <para>Toggle Streamer Mode (Hide names).</para>
        /// </summary>
        ToggleStreamerMode,

        /// <summary>
        /// Abrir menú de Clan/Gremio.
        /// <para>Open Guild/Clan menu.</para>
        /// </summary>
        OpenGuildMenu,

        // --- Misc / Minigames ---
        // --- Varios / Minijuegos ---

        /// <summary>
        /// Lanzar caña de pescar.
        /// <para>Cast fishing line.</para>
        /// </summary>
        FishCast,

        /// <summary>
        /// Recoger sedal de pesca.
        /// <para>Reel in fishing line.</para>
        /// </summary>
        FishReel,

        /// <summary>
        /// Tocar nota de instrumento 1.
        /// <para>Play instrument note 1.</para>
        /// </summary>
        PlayNote1,

        /// <summary>
        /// Tocar nota de instrumento 2.
        /// <para>Play instrument note 2.</para>
        /// </summary>
        PlayNote2,

        /// <summary>
        /// Tocar nota de instrumento 3.
        /// <para>Play instrument note 3.</para>
        /// </summary>
        PlayNote3,

        /// <summary>
        /// Colocar marcador en el mapa.
        /// <para>Place marker on map.</para>
        /// </summary>
        MapPlaceMarker,

        /// <summary>
        /// Quitar marcador del mapa.
        /// <para>Remove marker from map.</para>
        /// </summary>
        MapRemoveMarker,

        /// <summary>
        /// Siguiente emisora de radio.
        /// <para>Change radio station next.</para>
        /// </summary>
        RadioNext,

        /// <summary>
        /// Emisora de radio anterior.
        /// <para>Change radio station previous.</para>
        /// </summary>
        RadioPrev,

        /// <summary>
        /// Encender/Apagar radio.
        /// <para>Turn radio on/off.</para>
        /// </summary>
        RadioToggle,
        // --- Movimiento Básico ---
        /// <summary>
        /// Acción de moverse hacia arriba.
        /// <para>Move up action.</para>
        /// </summary>
        MoverArriba,
        /// <summary>
        /// Acción de moverse hacia abajo.
        /// <para>Move down action.</para>
        /// </summary>
        MoverAbajo,
        /// <summary>
        /// Acción de moverse hacia la izquierda.
        /// <para>Move left action.</para>
        /// </summary>
        MoverIzquierda,
        /// <summary>
        /// Acción de moverse hacia la derecha.
        /// <para>Move right action.</para>
        /// </summary>
        MoverDerecha,
        /// <summary>
        /// Acción de saltar.
        /// <para>Jump action.</para>
        /// </summary>
        Saltar,
        /// <summary>
        /// Acción de realizar un impulso rápido (Dash).
        /// <para>Dash action.</para>
        /// </summary>
        Impulso,

        // --- Control de Cámara ---
        /// <summary>
        /// Rotar la cámara hacia la izquierda.
        /// <para>Rotate camera left action.</para>
        /// </summary>
        RotarCamaraIzquierda,
        /// <summary>
        /// Rotar la cámara hacia la derecha.
        /// <para>Rotate camera right action.</para>
        /// </summary>
        RotarCamaraDerecha,
        /// <summary>
        /// Rotar la cámara hacia arriba.
        /// <para>Rotate camera up action.</para>
        /// </summary>
        RotarCamaraArriba,
        /// <summary>
        /// Rotar la cámara hacia abajo.
        /// <para>Rotate camera down action.</para>
        /// </summary>
        RotarCamaraAbajo,
        /// <summary>
        /// Acercar la cámara (Zoom In).
        /// <para>Zoom camera in.</para>
        /// </summary>
        AcercarCamara,
        /// <summary>
        /// Alejar la cámara (Zoom Out).
        /// <para>Zoom camera out.</para>
        /// </summary>
        AlejarCamara,
        /// <summary>
        /// Reiniciar la cámara a su posición por defecto.
        /// <para>Reset camera to default position.</para>
        /// </summary>
        ReiniciarCamara,
        /// <summary>
        /// Fijar la cámara en el objetivo.
        /// <para>Lock camera on target.</para>
        /// </summary>
        FijarObjetivo,
        /// <summary>
        /// Alternar entre vista de primera y tercera persona.
        /// <para>Toggle between first and third person view.</para>
        /// </summary>
        AlternarPerspectiva,
        /// <summary>
        /// Mirar hacia atrás rápidamente.
        /// <para>Look behind action.</para>
        /// </summary>
        MirarAtras,

        // --- Movimiento Avanzado ---
        /// <summary>
        /// Acción de esprintar o correr rápido.
        /// <para>Sprint action.</para>
        /// </summary>
        Esprintar,
        /// <summary>
        /// Alternar entre caminar y correr.
        /// <para>Toggle walking mode.</para>
        /// </summary>
        AlternarCaminar,
        /// <summary>
        /// Acción de agacharse.
        /// <para>Crouch action.</para>
        /// </summary>
        Agacharse,
        /// <summary>
        /// Acción de tumbarse o cuerpo a tierra.
        /// <para>Prone action (lie down).</para>
        /// </summary>
        Tumbarse,
        /// <summary>
        /// Deslizarse por el suelo.
        /// <para>Slide action.</para>
        /// </summary>
        Deslizarse,
        /// <summary>
        /// Correr por la pared.
        /// <para>Wall run action.</para>
        /// </summary>
        CorrerPorPared,
        /// <summary>
        /// Escalar hacia arriba.
        /// <para>Climb up action.</para>
        /// </summary>
        EscalarArriba,
        /// <summary>
        /// Descender escalando.
        /// <para>Climb down action.</para>
        /// </summary>
        EscalarAbajo,
        /// <summary>
        /// Nadar hacia arriba o salir a la superficie.
        /// <para>Swim up / Surface.</para>
        /// </summary>
        NadarArriba,
        /// <summary>
        /// Bucear o nadar hacia el fondo.
        /// <para>Dive / Swim down.</para>
        /// </summary>
        NadarAbajo,
        /// <summary>
        /// Planear en el aire.
        /// <para>Glide action.</para>
        /// </summary>
        Planear,
        /// <summary>
        /// Rodar para esquivar.
        /// <para>Dodge roll action.</para>
        /// </summary>
        RodarEsquivar,
        /// <summary>
        /// Usar el gancho de agarre.
        /// <para>Grappling hook action.</para>
        /// </summary>
        UsarGancho,

        // --- Combate y Armas ---
        /// <summary>
        /// Disparo o ataque principal.
        /// <para>Primary fire action.</para>
        /// </summary>
        DisparoPrincipal,
        /// <summary>
        /// Disparo o ataque secundario.
        /// <para>Secondary fire action.</para>
        /// </summary>
        DisparoSecundario,
        /// <summary>
        /// Acción de atacar (Genérico).
        /// <para>Attack action.</para>
        /// </summary>
        Atacar,
        /// <summary>
        /// Recargar el arma.
        /// <para>Reload weapon.</para>
        /// </summary>
        Recargar,
        /// <summary>
        /// Realizar un ataque pesado.
        /// <para>Heavy attack action.</para>
        /// </summary>
        AtaquePesado,
        /// <summary>
        /// Bloquear o defenderse.
        /// <para>Block or defend action.</para>
        /// </summary>
        Bloquear,
        /// <summary>
        /// Realizar un desvío (Parry).
        /// <para>Parry action.</para>
        /// </summary>
        Desvio,
        /// <summary>
        /// Dar una patada.
        /// <para>Kick action.</para>
        /// </summary>
        Patear,
        /// <summary>
        /// Apuntar con la mira.
        /// <para>Aim down sights (ADS).</para>
        /// </summary>
        ApuntarConMira,
        /// <summary>
        /// Lanzar una granada.
        /// <para>Throw grenade.</para>
        /// </summary>
        LanzarGranada,
        /// <summary>
        /// Cambiar a la siguiente arma.
        /// <para>Switch to next weapon.</para>
        /// </summary>
        ArmaSiguiente,
        /// <summary>
        /// Cambiar al arma anterior.
        /// <para>Switch to previous weapon.</para>
        /// </summary>
        ArmaAnterior,
        /// <summary>
        /// Enfundar o desenfundar el arma.
        /// <para>Holster or unholster weapon.</para>
        /// </summary>
        EnfundarArma,
        /// <summary>
        /// Inspeccionar el arma actual.
        /// <para>Inspect current weapon.</para>
        /// </summary>
        InspeccionarArma,
        /// <summary>
        /// Cambiar modo de disparo.
        /// <para>Change firing mode (Single, Burst, Auto).</para>
        /// </summary>
        CambiarModoDisparo,
        /// <summary>
        /// Equipar arma de la ranura 1.
        /// <para>Equip weapon in slot 1.</para>
        /// </summary>
        EquiparRanura1,
        /// <summary>
        /// Equipar arma de la ranura 2.
        /// <para>Equip weapon in slot 2.</para>
        /// </summary>
        EquiparRanura2,
        /// <summary>
        /// Equipar arma de la ranura 3.
        /// <para>Equip weapon in slot 3.</para>
        /// </summary>
        EquiparRanura3,
        /// <summary>
        /// Equipar arma de la ranura 4.
        /// <para>Equip weapon in slot 4.</para>
        /// </summary>
        EquiparRanura4,
        /// <summary>
        /// Ejecutar un remate o ejecución.
        /// <para>Use melee execution / finisher.</para>
        /// </summary>
        Ejecucion,

        // --- Interacción y Objetos ---
        /// <summary>
        /// Interacción general.
        /// <para>General interaction (Talk, Open, etc.).</para>
        /// </summary>
        Interactuar,
        /// <summary>
        /// Recoger un objeto.
        /// <para>Pick up item.</para>
        /// </summary>
        Recoger,
        /// <summary>
        /// Soltar el objeto actual.
        /// <para>Drop current item.</para>
        /// </summary>
        SoltarObjeto,
        /// <summary>
        /// Usar el objeto seleccionado.
        /// <para>Use currently selected item.</para>
        /// </summary>
        UsarObjeto,
        /// <summary>
        /// Usar curación.
        /// <para>Use health potion / medkit.</para>
        /// </summary>
        Curarse,
        /// <summary>
        /// Restaurar energía o maná.
        /// <para>Use mana potion / energy pack.</para>
        /// </summary>
        RestaurarEnergia,
        /// <summary>
        /// Encender o apagar linterna.
        /// <para>Toggle flashlight.</para>
        /// </summary>
        Linterna,
        /// <summary>
        /// Alternar visión nocturna.
        /// <para>Toggle night vision.</para>
        /// </summary>
        VisionNocturna,
        /// <summary>
        /// Escanear el entorno.
        /// <para>Scan environment.</para>
        /// </summary>
        Escanear,

        // --- Interacción Avanzada y Puzles ---
        /// <summary>
        /// Examinar detalle.
        /// <para>Examine detailed view of an object.</para>
        /// </summary>
        ExaminarDetalle,
        /// <summary>
        /// Empujar objeto.
        /// <para>Push object forward.</para>
        /// </summary>
        EmpujarObjeto,
        /// <summary>
        /// Tirar de objeto.
        /// <para>Pull object backward.</para>
        /// </summary>
        TirarObjeto,
        /// <summary>
        /// Encender fuego.
        /// <para>Ignite torch or light source.</para>
        /// </summary>
        Encender,
        /// <summary>
        /// Apagar fuego.
        /// <para>Extinguish fire or light source.</para>
        /// </summary>
        Extinguir,
        /// <summary>
        /// Hackear terminal.
        /// <para>Hack terminal or computer.</para>
        /// </summary>
        Hackear,
        /// <summary>
        /// Ganzuar cerradura.
        /// <para>Pick lock.</para>
        /// </summary>
        Ganzuar,
        /// <summary>
        /// Combinar objetos.
        /// <para>Combine items in crafting menu.</para>
        /// </summary>
        CombinarObjetos,
        /// <summary>
        /// Desmontar objeto.
        /// <para>Disassemble item for parts.</para>
        /// </summary>
        Desmontar,
        /// <summary>
        /// Rotar puzle horario.
        /// <para>Rotate puzzle piece clockwise.</para>
        /// </summary>
        PuzleRotarHorario,
        /// <summary>
        /// Rotar puzle antihorario.
        /// <para>Rotate puzzle piece counter-clockwise.</para>
        /// </summary>
        PuzleRotarAntihorario,

        // --- Gestión de Inventario e UI ---
        /// <summary>
        /// Mover al alijo.
        /// <para>Move item to stash/chest.</para>
        /// </summary>
        MoverAlAlijo,
        /// <summary>
        /// Tomar todo.
        /// <para>Take all items from container.</para>
        /// </summary>
        TomarTodo,
        /// <summary>
        /// Dividir pila.
        /// <para>Split item stack.</para>
        /// </summary>
        DividirPila,
        /// <summary>
        /// Marcar objeto.
        /// <para>Mark item as junk/favorite.</para>
        /// </summary>
        MarcarObjeto,
        /// <summary>
        /// Comparar objeto.
        /// <para>Compare item with equipped.</para>
        /// </summary>
        CompararObjeto,
        /// <summary>
        /// Ordenar por nombre.
        /// <para>Sort inventory by name.</para>
        /// </summary>
        OrdenarPorNombre,
        /// <summary>
        /// Ordenar por valor.
        /// <para>Sort inventory by weight/value.</para>
        /// </summary>
        OrdenarPorValor,
        /// <summary>
        /// Leer página siguiente.
        /// <para>Read next page (book/note).</para>
        /// </summary>
        LeerPaginaSiguiente,
        /// <summary>
        /// Leer página anterior.
        /// <para>Read previous page (book/note).</para>
        /// </summary>
        LeerPaginaAnterior,

        // --- Habilidades y Magia ---
        /// <summary>
        /// Habilidad 1.
        /// <para>Activate Skill 1.</para>
        /// </summary>
        Habilidad1,
        /// <summary>
        /// Habilidad 2.
        /// <para>Activate Skill 2.</para>
        /// </summary>
        Habilidad2,
        /// <summary>
        /// Habilidad 3.
        /// <para>Activate Skill 3.</para>
        /// </summary>
        Habilidad3,
        /// <summary>
        /// Habilidad 4.
        /// <para>Activate Skill 4.</para>
        /// </summary>
        Habilidad4,
        /// <summary>
        /// Habilidad Definitiva.
        /// <para>Activate Ultimate Ability.</para>
        /// </summary>
        HabilidadDefinitiva,
        /// <summary>
        /// Lanzar hechizo.
        /// <para>Cast selected spell.</para>
        /// </summary>
        LanzarHechizo,
        /// <summary>
        /// Cancelar lanzamiento.
        /// <para>Cancel current casting.</para>
        /// </summary>
        CancelarLanzamiento,
        /// <summary>
        /// Invocar montura.
        /// <para>Summon mount or vehicle.</para>
        /// </summary>
        InvocarMontura,

        // --- Interfaz de Usuario (UI) ---
        /// <summary>
        /// Menú de pausa.
        /// <para>Toggle Pause Menu.</para>
        /// </summary>
        MenuPausa,
        /// <summary>
        /// Abrir inventario.
        /// <para>Open Inventory.</para>
        /// </summary>
        AbrirInventario,
        /// <summary>
        /// Abrir mapa.
        /// <para>Open Map.</para>
        /// </summary>
        AbrirMapa,
        /// <summary>
        /// Alternar tamaño minimapa.
        /// <para>Toggle mini-map size (small/large).</para>
        /// </summary>
        AlternarTamanoMinimapa,
        /// <summary>
        /// Abrir diario.
        /// <para>Open Quest Log / Journal.</para>
        /// </summary>
        AbrirDiario,
        /// <summary>
        /// Fijar misión.
        /// <para>Pin quest to HUD.</para>
        /// </summary>
        FijarMision,
        /// <summary>
        /// Abrir árbol de habilidades.
        /// <para>Open Skill Tree.</para>
        /// </summary>
        AbrirArbolHabilidades,
        /// <summary>
        /// Abrir ficha de personaje.
        /// <para>Open Character Sheet / Stats.</para>
        /// </summary>
        AbrirFichaPersonaje,
        /// <summary>
        /// UI Aceptar.
        /// <para>UI Submit / Accept.</para>
        /// </summary>
        UIAceptar,
        /// <summary>
        /// UI Cancelar.
        /// <para>UI Cancel / Back.</para>
        /// </summary>
        UICancelar,
        /// <summary>
        /// Guardado rápido.
        /// <para>Quick Save game.</para>
        /// </summary>
        GuardadoRapido,
        /// <summary>
        /// Carga rápida.
        /// <para>Quick Load game.</para>
        /// </summary>
        CargaRapida,
        /// <summary>
        /// Captura de pantalla.
        /// <para>Take screenshot.</para>
        /// </summary>
        CapturaPantalla,
        /// <summary>
        /// Alternar HUD.
        /// <para>Toggle HUD visibility.</para>
        /// </summary>
        AlternarHUD,
        /// <summary>
        /// Consola de depuración.
        /// <para>Toggle Debug Console.</para>
        /// </summary>
        ConsolaDepuracion,

        // --- Vehículos Terrestres ---
        /// <summary>
        /// Acelerar vehículo.
        /// <para>Vehicle Accelerate / Throttle.</para>
        /// </summary>
        VehiculoAcelerar,
        /// <summary>
        /// Frenar vehículo.
        /// <para>Vehicle Brake / Reverse.</para>
        /// </summary>
        VehiculoFrenar,
        /// <summary>
        /// Girar vehículo izquierda.
        /// <para>Vehicle Turn Left.</para>
        /// </summary>
        VehiculoGirarIzquierda,
        /// <summary>
        /// Girar vehículo derecha.
        /// <para>Vehicle Turn Right.</para>
        /// </summary>
        VehiculoGirarDerecha,
        /// <summary>
        /// Freno de mano vehículo.
        /// <para>Vehicle Handbrake / Drift.</para>
        /// </summary>
        VehiculoFrenoMano,
        /// <summary>
        /// Turbo vehículo.
        /// <para>Vehicle Boost / Nitro.</para>
        /// </summary>
        VehiculoTurbo,
        /// <summary>
        /// Bocina vehículo.
        /// <para>Use Vehicle Horn.</para>
        /// </summary>
        VehiculoBocina,
        /// <summary>
        /// Luces vehículo.
        /// <para>Toggle Vehicle Lights.</para>
        /// </summary>
        VehiculoLuces,
        /// <summary>
        /// Salir vehículo.
        /// <para>Exit Vehicle.</para>
        /// </summary>
        VehiculoSalir,

        // --- Vuelo y Espacio (6DOF) ---
        /// <summary>
        /// Guiñada izquierda (Vuelo).
        /// <para>Yaw left (Flight).</para>
        /// </summary>
        VueloGuinadaIzquierda,
        /// <summary>
        /// Guiñada derecha (Vuelo).
        /// <para>Yaw right (Flight).</para>
        /// </summary>
        VueloGuinadaDerecha,
        /// <summary>
        /// Alabeo izquierda (Vuelo).
        /// <para>Roll left (Flight).</para>
        /// </summary>
        VueloAlabeoIzquierda,
        /// <summary>
        /// Alabeo derecha (Vuelo).
        /// <para>Roll right (Flight).</para>
        /// </summary>
        VueloAlabeoDerecha,
        /// <summary>
        /// Cabeceo arriba (Vuelo).
        /// <para>Pitch up (Flight).</para>
        /// </summary>
        VueloCabeceoArriba,
        /// <summary>
        /// Cabeceo abajo (Vuelo).
        /// <para>Pitch down (Flight).</para>
        /// </summary>
        VueloCabeceoAbajo,
        /// <summary>
        /// Empuje vertical arriba.
        /// <para>Vertical thrust up (VTOL/Space).</para>
        /// </summary>
        EmpujeVerticalArriba,
        /// <summary>
        /// Empuje vertical abajo.
        /// <para>Vertical thrust down (VTOL/Space).</para>
        /// </summary>
        EmpujeVerticalAbajo,
        /// <summary>
        /// Empuje lateral izquierda.
        /// <para>Lateral thrust left (Space).</para>
        /// </summary>
        EmpujeLateralIzquierda,
        /// <summary>
        /// Empuje lateral derecha.
        /// <para>Lateral thrust right (Space).</para>
        /// </summary>
        EmpujeLateralDerecha,
        /// <summary>
        /// Alternar tren de aterrizaje.
        /// <para>Toggle landing gear.</para>
        /// </summary>
        TrenAterrizaje,
        /// <summary>
        /// Activar hipervelocidad.
        /// <para>Engage warp drive / hyperdrive.</para>
        /// </summary>
        ActivarHipervelocidad,
        /// <summary>
        /// Desplegar contramedidas.
        /// <para>Deploy countermeasures (flares/chaff).</para>
        /// </summary>
        DesplegarContramedidas,
        /// <summary>
        /// Objetivo enemigo más cercano.
        /// <para>Target nearest enemy.</para>
        /// </summary>
        ObjetivoEnemigoCercano,
        /// <summary>
        /// Ciclar objetivos.
        /// <para>Cycle hostile targets.</para>
        /// </summary>
        CiclarObjetivos,
        /// <summary>
        /// Ciclar subsistemas.
        /// <para>Cycle sub-systems targeting (engines, weapons).</para>
        /// </summary>
        CiclarSubsistemas,

        // --- Multijugador y Social ---
        /// <summary>
        /// Pulsar para hablar.
        /// <para>Push to talk.</para>
        /// </summary>
        PulsarParaHablar,
        /// <summary>
        /// Abrir chat.
        /// <para>Open Text Chat.</para>
        /// </summary>
        AbrirChat,
        /// <summary>
        /// Mostrar puntuaciones.
        /// <para>Show Scoreboard.</para>
        /// </summary>
        MostrarPuntuaciones,
        /// <summary>
        /// Marcar ubicación (Ping).
        /// <para>Mark/Ping location contextually.</para>
        /// </summary>
        MarcarUbicacion,
        /// <summary>
        /// Marcar enemigo.
        /// <para>Mark/Ping enemy.</para>
        /// </summary>
        MarcarEnemigo,
        /// <summary>
        /// Emote: Saludar.
        /// <para>Emote: Wave.</para>
        /// </summary>
        GestoSaludar,
        /// <summary>
        /// Emote: Bailar.
        /// <para>Emote: Dance.</para>
        /// </summary>
        GestoBailar,
        /// <summary>
        /// Emote: Burlarse.
        /// <para>Emote: Taunt.</para>
        /// </summary>
        GestoBurlarse,
        /// <summary>
        /// Votar Sí.
        /// <para>Vote Yes / Agree.</para>
        /// </summary>
        VotarSi,
        /// <summary>
        /// Votar No.
        /// <para>Vote No / Disagree.</para>
        /// </summary>
        VotarNo,
        /// <summary>
        /// Silenciar todos.
        /// <para>Mute all players.</para>
        /// </summary>
        SilenciarTodos,
        /// <summary>
        /// Reactivar audio todos.
        /// <para>Unmute all players.</para>
        /// </summary>
        ReactivarAudioTodos,
        /// <summary>
        /// Solicitud de amistad.
        /// <para>Send friend request to target.</para>
        /// </summary>
        SolicitudAmistad,
        /// <summary>
        /// Bloquear jugador.
        /// <para>Block target player.</para>
        /// </summary>
        BloquearJugador,
        /// <summary>
        /// Reportar jugador.
        /// <para>Report player behavior.</para>
        /// </summary>
        ReportarJugador,
        /// <summary>
        /// Modo Streamer.
        /// <para>Toggle Streamer Mode (Hide names).</para>
        /// </summary>
        ModoStreamer,
        /// <summary>
        /// Menú de clan.
        /// <para>Open Guild/Clan menu.</para>
        /// </summary>
        MenuClan,

        // --- Comandos de Escuadrón ---
        /// <summary>
        /// Escuadrón: Seguir.
        /// <para>Command squad: Follow me.</para>
        /// </summary>
        EscuadronSeguir,
        /// <summary>
        /// Escuadrón: Alto el fuego.
        /// <para>Command squad: Hold fire.</para>
        /// </summary>
        EscuadronAltoElFuego,
        /// <summary>
        /// Escuadrón: Fuego a discreción.
        /// <para>Command squad: Open fire.</para>
        /// </summary>
        EscuadronFuegoLibre,
        /// <summary>
        /// Escuadrón: Ir a posición.
        /// <para>Command squad: Go to point.</para>
        /// </summary>
        EscuadronIrA,
        /// <summary>
        /// Escuadrón: Reagruparse.
        /// <para>Command squad: Regroup.</para>
        /// </summary>
        EscuadronReagruparse,
        /// <summary>
        /// Escuadrón: Flanquear izquierda.
        /// <para>Command squad: Flank left.</para>
        /// </summary>
        EscuadronFlanquearIzq,
        /// <summary>
        /// Escuadrón: Flanquear derecha.
        /// <para>Command squad: Flank right.</para>
        /// </summary>
        EscuadronFlanquearDer,
        /// <summary>
        /// Escuadrón: Cubrirse.
        /// <para>Command squad: Take cover.</para>
        /// </summary>
        EscuadronCubrirse,
        /// <summary>
        /// Compañero: Habilidad especial.
        /// <para>Command companion: Use special ability.</para>
        /// </summary>
        CompaneroHabilidadEspecial,
        /// <summary>
        /// Cambiar personaje siguiente.
        /// <para>Swap control to next character.</para>
        /// </summary>
        CambiarPersonajeSiguiente,
        /// <summary>
        /// Cambiar personaje anterior.
        /// <para>Swap control to previous character.</para>
        /// </summary>
        CambiarPersonajeAnterior,

        // --- Estrategia y Construcción ---
        /// <summary>
        /// Rotar objeto izquierda.
        /// <para>Rotate placement object left.</para>
        /// </summary>
        RotarObjetoIzquierda,
        /// <summary>
        /// Rotar objeto derecha.
        /// <para>Rotate placement object right.</para>
        /// </summary>
        RotarObjetoDerecha,
        /// <summary>
        /// Confirmar construcción.
        /// <para>Confirm placement / build.</para>
        /// </summary>
        ConfirmarConstruccion,
        /// <summary>
        /// Cancelar construcción.
        /// <para>Cancel placement.</para>
        /// </summary>
        CancelarConstruccion,
        /// <summary>
        /// Seleccionar todas las unidades.
        /// <para>Select all units.</para>
        /// </summary>
        SeleccionarTodo,
        /// <summary>
        /// Comando: Mover.
        /// <para>Command: Move to.</para>
        /// </summary>
        ComandoMover,
        /// <summary>
        /// Comando: Atacar.
        /// <para>Command: Attack target.</para>
        /// </summary>
        ComandoAtacar,
        /// <summary>
        /// Comando: Mantener posición.
        /// <para>Command: Hold position.</para>
        /// </summary>
        ComandoMantener,

        // --- Modo Editor / Creativo ---
        /// <summary>
        /// Alternar Modo Foto.
        /// <para>Toggle Photo Mode.</para>
        /// </summary>
        ModoFoto,
        /// <summary>
        /// Aumentar campo de visión.
        /// <para>Increase Field of View (FOV).</para>
        /// </summary>
        AumentarFOV,
        /// <summary>
        /// Disminuir campo de visión.
        /// <para>Decrease Field of View (FOV).</para>
        /// </summary>
        DisminuirFOV,
        /// <summary>
        /// Aumentar profundidad de campo.
        /// <para>Increase Depth of Field (DOF).</para>
        /// </summary>
        AumentarDOF,
        /// <summary>
        /// Disminuir profundidad de campo.
        /// <para>Decrease Depth of Field (DOF).</para>
        /// </summary>
        DisminuirDOF,
        /// <summary>
        /// Alternar ajuste a rejilla.
        /// <para>Toggle Grid Snapping.</para>
        /// </summary>
        AjusteRejilla,
        /// <summary>
        /// Duplicar objeto.
        /// <para>Duplicate selected object.</para>
        /// </summary>
        DuplicarObjeto,
        /// <summary>
        /// Eliminar objeto.
        /// <para>Delete selected object.</para>
        /// </summary>
        EliminarObjeto,
        /// <summary>
        /// Deshacer acción.
        /// <para>Undo last action (Editor).</para>
        /// </summary>
        Deshacer,
        /// <summary>
        /// Rehacer acción.
        /// <para>Redo last action (Editor).</para>
        /// </summary>
        Rehacer,
        /// <summary>
        /// Guardar plano.
        /// <para>Save schematic/blueprint.</para>
        /// </summary>
        GuardarPlano,
        /// <summary>
        /// Cargar plano.
        /// <para>Load schematic/blueprint.</para>
        /// </summary>
        CargarPlano,

        // --- Realidad Virtual (VR) ---
        /// <summary>
        /// Recentrar VR.
        /// <para>Recenter VR View.</para>
        /// </summary>
        VRRecentrar,
        /// <summary>
        /// Teletransporte VR.
        /// <para>Teleport (VR Locomotion).</para>
        /// </summary>
        VRTeletransporte,
        /// <summary>
        /// Agarre mano izquierda.
        /// <para>Grip Left Hand.</para>
        /// </summary>
        VRAgarreIzquierda,
        /// <summary>
        /// Agarre mano derecha.
        /// <para>Grip Right Hand.</para>
        /// </summary>
        VRAgarreDerecha,
        /// <summary>
        /// Gatillo mano izquierda.
        /// <para>Trigger Left Hand.</para>
        /// </summary>
        VRGatilloIzquierda,
        /// <summary>
        /// Gatillo mano derecha.
        /// <para>Trigger Right Hand.</para>
        /// </summary>
        VRGatilloDerecha,
        /// <summary>
        /// Giro rápido izquierda.
        /// <para>Snap Turn Left (VR).</para>
        /// </summary>
        VRGiroRapidoIzquierda,
        /// <summary>
        /// Giro rápido derecha.
        /// <para>Snap Turn Right (VR).</para>
        /// </summary>
        VRGiroRapidoDerecha,

        // --- Depuración y Desarrollador ---
        /// <summary>
        /// Modo Dios.
        /// <para>Toggle God Mode.</para>
        /// </summary>
        ModoDios,
        /// <summary>
        /// Modo NoClip (Atravesar).
        /// <para>Toggle NoClip mode.</para>
        /// </summary>
        ModoNoClip,
        /// <summary>
        /// Ralentizar tiempo.
        /// <para>Slow down game time.</para>
        /// </summary>
        TiempoLento,
        /// <summary>
        /// Acelerar tiempo.
        /// <para>Speed up game time.</para>
        /// </summary>
        TiempoRapido,

        // --- Varios ---
        /// <summary>
        /// Lanzar caña de pescar.
        /// <para>Cast fishing line.</para>
        /// </summary>
        PescaLanzar,
        /// <summary>
        /// Recoger caña de pescar.
        /// <para>Reel in fishing line.</para>
        /// </summary>
        PescaRecoger,
        /// <summary>
        /// Tocar nota 1.
        /// <para>Play instrument note 1.</para>
        /// </summary>
        TocarNota1,
        /// <summary>
        /// Tocar nota 2.
        /// <para>Play instrument note 2.</para>
        /// </summary>
        TocarNota2,
        /// <summary>
        /// Tocar nota 3.
        /// <para>Play instrument note 3.</para>
        /// </summary>
        TocarNota3,
        /// <summary>
        /// Poner marcador mapa.
        /// <para>Place marker on map.</para>
        /// </summary>
        MapaPonerMarcador,
        /// <summary>
        /// Quitar marcador mapa.
        /// <para>Remove marker from map.</para>
        /// </summary>
        MapaQuitarMarcador,
        /// <summary>
        /// Radio siguiente.
        /// <para>Change radio station next.</para>
        /// </summary>
        RadioSiguiente,
        /// <summary>
        /// Radio anterior.
        /// <para>Change radio station previous.</para>
        /// </summary>
        RadioAnterior,
        /// <summary>
        /// Radio encender/apagar.
        /// <para>Turn radio on/off.</para>
        /// </summary>
        RadioInterruptor
    }
}
