// YTBEditor - Script principal
// Editor visual para archivos YotsubaGame.ytb

// Nombre del archivo YTB a cargar automáticamente
const DEFAULT_YTB_FILE = 'YotsubaGame.ytb';

const COMPONENT_TEMPLATES = {
    TransformComponent: [
        { Item1: "Position", Item2: "0,0,0" },
        { Item1: "Size", Item2: "0,0,0" },
        { Item1: "Color", Item2: "White" },
        { Item1: "SpriteEffects", Item2: "None" },
        { Item1: "Scale", Item2: "1" }
    ],
    SpriteComponent2D: [
        { Item1: "TextureAtlasPath", Item2: "" },
        { Item1: "SpriteName", Item2: "" },
        { Item1: "SourceRectangle", Item2: "0,0,0,0" },
        { Item1: "IsVisible", Item2: "true" }
    ],
    AnimationComponent2D: [
        { Item1: "TextureAtlasPath", Item2: "" },
        { Item1: "AnimationBindings", Item2: "" },
        { Item1: "CurrentAnimationType", Item2: "" }
    ],
    RigidBodyComponent2D: [
        { Item1: "OffSetCollision", Item2: "0,0" },
        { Item1: "Velocity", Item2: "0,0" },
        { Item1: "GameType", Item2: "TopDown" },
        { Item1: "Mass", Item2: "0" }
    ],
    ButtonComponent2D: [
        { Item1: "IsActive", Item2: "false" },
        { Item1: "EffectiveArea", Item2: "0,0,0,0" },
        { Item1: "Description", Item2: "" }
    ],
    InputComponent: [
        { Item1: "InputsInUse", Item2: "" },
        { Item1: "GamePadIndex", Item2: "" },
        { Item1: "KeyboardMappings", Item2: "" },
        { Item1: "MouseMappings", Item2: "" }
    ],
    ScriptComponent: [
        { Item1: "Scripts", Item2: "CSHARP&:&RouteToScript&;&" }
    ],
    TileMapComponent2D: [
        { Item1: "TileMapPath", Item2: "nothing" }
    ],
    FontComponent2D: [
        { Item1: "Texto", Item2: "" },
        { Item1: "Font", Item2: "" }
    ],
    // Componente de cámara 3D basado en EntityYTBXmlTemplate.CameraTemplate
    CameraComponent3D: [
        { Item1: "EntityName", Item2: "" },
        { Item1: "InitialPosition", Item2: "0,60,30" },
        { Item1: "AngleView", Item2: "60" },
        { Item1: "NearRender", Item2: "10" },
        { Item1: "FarRender", Item2: "3000" }
    ],
    // Componente de shader basado en EntityYTBXmlTemplate.ShaderTemplate
    ShaderComponent: [
        { Item1: "ShaderPath", Item2: "" },
        { Item1: "IsActive", Item2: "true" },
        { Item1: "params", Item2: "" }
    ]
};

let gameData = { scene: [] };
let currentSceneIndex = -1;

// Elementos del DOM
const elements = {
    sceneList: document.getElementById('sceneList'),
    welcomePanel: document.getElementById('welcomePanel'),
    sceneEditor: document.getElementById('sceneEditor'),
    sceneTitle: document.getElementById('sceneTitle'),
    sceneNameInput: document.getElementById('sceneNameInput'),
    entitiesContainer: document.getElementById('entitiesContainer'),
    modal: document.getElementById('modal'),
    modalTitle: document.getElementById('modalTitle'),
    modalBody: document.getElementById('modalBody'),
    modalConfirmBtn: document.getElementById('modalConfirmBtn'),
    modalCancelBtn: document.getElementById('modalCancelBtn'),
    closeModalBtn: document.getElementById('closeModalBtn'),
    toast: document.getElementById('toast'),
    fileInput: document.getElementById('fileInput')
};

// Inicialización
document.addEventListener('DOMContentLoaded', () => {
    setupEventListeners();
    tryLoadDefaultFile();
});

function setupEventListeners() {
    document.getElementById('loadBtn').addEventListener('click', () => elements.fileInput.click());
    document.getElementById('loadJsonBtn').addEventListener('click', () => elements.fileInput.click());
    document.getElementById('saveBtn').addEventListener('click', saveToFile);
    document.getElementById('exportBtn').addEventListener('click', exportJson);
    document.getElementById('addSceneBtn').addEventListener('click', addScene);
    document.getElementById('addEntityBtn').addEventListener('click', addEntity);
    document.getElementById('deleteSceneBtn').addEventListener('click', deleteCurrentScene);
    
    elements.fileInput.addEventListener('change', handleFileLoad);
    elements.sceneNameInput.addEventListener('input', updateSceneName);
    
    elements.closeModalBtn.addEventListener('click', closeModal);
    elements.modalCancelBtn.addEventListener('click', closeModal);
}

async function tryLoadDefaultFile() {
    // Intentar cargar el archivo .ytb del mismo directorio
    const filesToTry = [
        DEFAULT_YTB_FILE,
        `./${DEFAULT_YTB_FILE}`,
        `${window.location.pathname.substring(0, window.location.pathname.lastIndexOf('/'))}/${DEFAULT_YTB_FILE}`
    ];
    
    for (const filePath of filesToTry) {
        try {
            const response = await fetch(filePath);
            if (response.ok) {
                const text = await response.text();
                gameData = JSON.parse(text);
                renderSceneList();
                showToast(`? ${DEFAULT_YTB_FILE} cargado automáticamente`);
                
                // Seleccionar la primera escena si existe
                if (gameData.scene && gameData.scene.length > 0) {
                    selectScene(0);
                }
                return; // Salir si se cargó correctamente
            }
        } catch (e) {
            console.log(`Intento fallido para ${filePath}:`, e);
        }
    }
    
    console.log('No se pudo cargar el archivo YTB por defecto. El usuario puede cargarlo manualmente.');
}

function handleFileLoad(e) {
    const file = e.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = (event) => {
        try {
            gameData = JSON.parse(event.target.result);
            renderSceneList();
            showToast(`? ${file.name} cargado correctamente`);
            
            if (gameData.scene.length > 0) {
                selectScene(0);
            }
        } catch (err) {
            showToast('? Error al parsear el archivo JSON', true);
            console.error(err);
        }
    };
    reader.readAsText(file);
}

function saveToFile() {
    const json = JSON.stringify(gameData, null, 2);
    const blob = new Blob([json], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    
    const a = document.createElement('a');
    a.href = url;
    a.download = 'YotsubaGame.ytb';
    a.click();
    
    URL.revokeObjectURL(url);
    showToast('?? Archivo guardado como YotsubaGame.ytb');
}

function exportJson() {
    const json = JSON.stringify(gameData, null, 2);
    navigator.clipboard.writeText(json).then(() => {
        showToast('?? JSON copiado al portapapeles');
    }).catch(() => {
        showToast('? Error al copiar al portapapeles', true);
    });
}

function renderSceneList() {
    elements.sceneList.innerHTML = '';
    
    gameData.scene.forEach((scene, index) => {
        const li = document.createElement('li');
        li.className = `scene-item${index === currentSceneIndex ? ' active' : ''}`;
        li.innerHTML = `
            <span class="scene-icon">??</span>
            <span class="scene-name">${scene.name}</span>
            <span class="entity-count">${scene.entities?.length || 0}</span>
        `;
        li.addEventListener('click', () => selectScene(index));
        elements.sceneList.appendChild(li);
    });
}

function selectScene(index) {
    currentSceneIndex = index;
    const scene = gameData.scene[index];
    
    elements.welcomePanel.classList.add('hidden');
    elements.sceneEditor.classList.remove('hidden');
    
    elements.sceneTitle.textContent = `?? ${scene.name}`;
    elements.sceneNameInput.value = scene.name;
    
    renderSceneList();
    renderEntities();
}

function updateSceneName(e) {
    if (currentSceneIndex < 0) return;
    
    gameData.scene[currentSceneIndex].name = e.target.value;
    elements.sceneTitle.textContent = `?? ${e.target.value}`;
    renderSceneList();
}

function renderEntities() {
    const scene = gameData.scene[currentSceneIndex];
    if (!scene.entities) scene.entities = [];
    
    elements.entitiesContainer.innerHTML = '';
    
    scene.entities.forEach((entity, entityIndex) => {
        const card = createEntityCard(entity, entityIndex);
        elements.entitiesContainer.appendChild(card);
    });
}

function createEntityCard(entity, entityIndex) {
    const card = document.createElement('div');
    card.className = 'entity-card';
    card.dataset.entityIndex = entityIndex;
    
    card.innerHTML = `
        <div class="entity-header">
            <div class="entity-title">
                <span class="expand-icon">?</span>
                <input type="text" value="${escapeHtml(entity.name)}" 
                       onchange="updateEntityName(${entityIndex}, this.value)"
                       onclick="event.stopPropagation()">
            </div>
            <div class="entity-actions">
                <button class="btn btn-small btn-danger" onclick="deleteEntity(${entityIndex}); event.stopPropagation();">
                    ???
                </button>
            </div>
        </div>
        <div class="entity-body">
            ${renderComponents(entity.components || [], entityIndex)}
            <div class="add-component-section">
                <div class="component-select">
                    <select id="componentSelect_${entityIndex}">
                        ${Object.keys(COMPONENT_TEMPLATES).map(name => 
                            `<option value="${name}">${name}</option>`
                        ).join('')}
                    </select>
                    <button class="btn btn-small btn-success" onclick="addComponent(${entityIndex})">
                        + Componente
                    </button>
                </div>
            </div>
        </div>
    `;
    
    const header = card.querySelector('.entity-header');
    header.addEventListener('click', () => {
        card.classList.toggle('collapsed');
    });
    
    return card;
}

function renderComponents(components, entityIndex) {
    return components.map((component, componentIndex) => `
        <div class="component-card" data-component-index="${componentIndex}">
            <div class="component-header" onclick="toggleComponent(this)">
                <div class="component-title">
                    <span class="expand-icon">?</span>
                    <span>?? ${component.ComponentName}</span>
                </div>
                <button class="btn btn-small btn-danger btn-icon" 
                        onclick="deleteComponent(${entityIndex}, ${componentIndex}); event.stopPropagation();">
                    ???
                </button>
            </div>
            <div class="component-body">
                ${renderProperties(component.properties || [], entityIndex, componentIndex)}
            </div>
        </div>
    `).join('');
}

function renderProperties(properties, entityIndex, componentIndex) {
    return properties.map((prop, propIndex) => `
        <div class="property-row">
            <label class="property-label">${prop.Item1}</label>
            <input type="text" class="input-field" 
                   value="${escapeHtml(prop.Item2)}"
                   onchange="updateProperty(${entityIndex}, ${componentIndex}, ${propIndex}, this.value)">
        </div>
    `).join('');
}

function toggleComponent(header) {
    header.parentElement.classList.toggle('collapsed');
}

function updateEntityName(entityIndex, newName) {
    gameData.scene[currentSceneIndex].entities[entityIndex].name = newName;
    showToast('?? Nombre de entidad actualizado');
}

function updateProperty(entityIndex, componentIndex, propIndex, newValue) {
    gameData.scene[currentSceneIndex].entities[entityIndex]
        .components[componentIndex].properties[propIndex].Item2 = newValue;
}

function addScene() {
    const newScene = {
        name: `Nueva Escena ${gameData.scene.length + 1}`,
        entities: []
    };
    
    gameData.scene.push(newScene);
    renderSceneList();
    selectScene(gameData.scene.length - 1);
    showToast('?? Nueva escena creada');
}

function deleteCurrentScene() {
    if (currentSceneIndex < 0) return;
    
    showConfirmModal(
        '¿Eliminar escena?',
        `¿Estás seguro de que deseas eliminar la escena "${gameData.scene[currentSceneIndex].name}"? Esta acción no se puede deshacer.`,
        () => {
            gameData.scene.splice(currentSceneIndex, 1);
            currentSceneIndex = -1;
            
            elements.sceneEditor.classList.add('hidden');
            elements.welcomePanel.classList.remove('hidden');
            
            renderSceneList();
            showToast('??? Escena eliminada');
        }
    );
}

function addEntity() {
    if (currentSceneIndex < 0) return;
    
    const scene = gameData.scene[currentSceneIndex];
    if (!scene.entities) scene.entities = [];
    
    const newEntity = {
        name: `Entidad ${scene.entities.length + 1}`,
        components: [
            {
                ComponentName: 'TransformComponent',
                properties: [...COMPONENT_TEMPLATES.TransformComponent]
            }
        ]
    };
    
    scene.entities.push(newEntity);
    renderEntities();
    renderSceneList();
    showToast('?? Nueva entidad creada');
}

function deleteEntity(entityIndex) {
    showConfirmModal(
        '¿Eliminar entidad?',
        `¿Estás seguro de que deseas eliminar esta entidad? Esta acción no se puede deshacer.`,
        () => {
            gameData.scene[currentSceneIndex].entities.splice(entityIndex, 1);
            renderEntities();
            renderSceneList();
            showToast('??? Entidad eliminada');
        }
    );
}

function addComponent(entityIndex) {
    const select = document.getElementById(`componentSelect_${entityIndex}`);
    const componentName = select.value;
    
    const entity = gameData.scene[currentSceneIndex].entities[entityIndex];
    if (!entity.components) entity.components = [];
    
    // Verificar si ya existe
    const exists = entity.components.some(c => c.ComponentName === componentName);
    if (exists) {
        showToast('?? Este componente ya existe en la entidad', true);
        return;
    }
    
    const newComponent = {
        ComponentName: componentName,
        properties: COMPONENT_TEMPLATES[componentName].map(p => ({ ...p }))
    };
    
    entity.components.push(newComponent);
    renderEntities();
    showToast(`?? ${componentName} agregado`);
}

function deleteComponent(entityIndex, componentIndex) {
    gameData.scene[currentSceneIndex].entities[entityIndex].components.splice(componentIndex, 1);
    renderEntities();
    showToast('??? Componente eliminado');
}

function showConfirmModal(title, message, onConfirm) {
    elements.modalTitle.textContent = title;
    elements.modalBody.innerHTML = `<p style="color: var(--text-secondary);">${message}</p>`;
    elements.modal.classList.remove('hidden');
    
    elements.modalConfirmBtn.onclick = () => {
        onConfirm();
        closeModal();
    };
}

function closeModal() {
    elements.modal.classList.add('hidden');
}

function showToast(message, isError = false) {
    elements.toast.textContent = message;
    elements.toast.className = `toast${isError ? ' error' : ''}`;
    elements.toast.classList.remove('hidden');
    
    setTimeout(() => {
        elements.toast.classList.add('hidden');
    }, 3000);
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text || '';
    return div.innerHTML;
}

// Exponer funciones globalmente para los event handlers inline
window.updateEntityName = updateEntityName;
window.updateProperty = updateProperty;
window.deleteEntity = deleteEntity;
window.addComponent = addComponent;
window.deleteComponent = deleteComponent;
window.toggleComponent = toggleComponent;
