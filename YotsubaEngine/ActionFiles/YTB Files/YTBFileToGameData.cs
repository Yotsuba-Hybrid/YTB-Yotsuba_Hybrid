

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YotsubaEngine.ActionFiles.TMX_Files.TiledCS;
using YotsubaEngine.Audio;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_3D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.Events.YTBEvents.EngineEvents;
using YotsubaEngine.Exceptions;
using YotsubaEngine.Graphics;
using YotsubaEngine.Graphics.Shaders;
using YotsubaEngine.HighestPerformanceTypes;
using YotsubaEngine.Templates;
using static YotsubaEngine.Core.Component.C_AGNOSTIC.RigidBody;
using static YotsubaEngine.Core.System.S_AGNOSTIC.InputSystem;
using static YotsubaEngine.Exceptions.GameWontRun;

namespace YotsubaEngine.ActionFiles.YTB_Files
{
    /// <summary>
    /// Contiene los métodos necesarios para transformar el archivo json en escenas y entidades en memoria en tiempo de ejecución.
    /// <para>Contains methods to transform JSON files into runtime scenes and entities.</para>
    /// </summary>
    public class YTBFileToGameData
    {
        /// <summary>
        /// Static dictionary mapping color names to XNA Color values for AOT-compatible color parsing.
        /// Diccionario estático que mapea nombres de colores a valores Color de XNA para parseo compatible con AOT.
        /// </summary>
        private static readonly Dictionary<string, Color> NamedColors = new(StringComparer.OrdinalIgnoreCase)
        {
            { "AliceBlue", Color.AliceBlue },
            { "AntiqueWhite", Color.AntiqueWhite },
            { "Aqua", Color.Aqua },
            { "Aquamarine", Color.Aquamarine },
            { "Azure", Color.Azure },
            { "Beige", Color.Beige },
            { "Bisque", Color.Bisque },
            { "Black", Color.Black },
            { "BlanchedAlmond", Color.BlanchedAlmond },
            { "Blue", Color.Blue },
            { "BlueViolet", Color.BlueViolet },
            { "Brown", Color.Brown },
            { "BurlyWood", Color.BurlyWood },
            { "CadetBlue", Color.CadetBlue },
            { "Chartreuse", Color.Chartreuse },
            { "Chocolate", Color.Chocolate },
            { "Coral", Color.Coral },
            { "CornflowerBlue", Color.CornflowerBlue },
            { "Cornsilk", Color.Cornsilk },
            { "Crimson", Color.Crimson },
            { "Cyan", Color.Cyan },
            { "DarkBlue", Color.DarkBlue },
            { "DarkCyan", Color.DarkCyan },
            { "DarkGoldenrod", Color.DarkGoldenrod },
            { "DarkGray", Color.DarkGray },
            { "DarkGreen", Color.DarkGreen },
            { "DarkKhaki", Color.DarkKhaki },
            { "DarkMagenta", Color.DarkMagenta },
            { "DarkOliveGreen", Color.DarkOliveGreen },
            { "DarkOrange", Color.DarkOrange },
            { "DarkOrchid", Color.DarkOrchid },
            { "DarkRed", Color.DarkRed },
            { "DarkSalmon", Color.DarkSalmon },
            { "DarkSeaGreen", Color.DarkSeaGreen },
            { "DarkSlateBlue", Color.DarkSlateBlue },
            { "DarkSlateGray", Color.DarkSlateGray },
            { "DarkTurquoise", Color.DarkTurquoise },
            { "DarkViolet", Color.DarkViolet },
            { "DeepPink", Color.DeepPink },
            { "DeepSkyBlue", Color.DeepSkyBlue },
            { "DimGray", Color.DimGray },
            { "DodgerBlue", Color.DodgerBlue },
            { "Firebrick", Color.Firebrick },
            { "FloralWhite", Color.FloralWhite },
            { "ForestGreen", Color.ForestGreen },
            { "Fuchsia", Color.Fuchsia },
            { "Gainsboro", Color.Gainsboro },
            { "GhostWhite", Color.GhostWhite },
            { "Gold", Color.Gold },
            { "Goldenrod", Color.Goldenrod },
            { "Gray", Color.Gray },
            { "Green", Color.Green },
            { "GreenYellow", Color.GreenYellow },
            { "Honeydew", Color.Honeydew },
            { "HotPink", Color.HotPink },
            { "IndianRed", Color.IndianRed },
            { "Indigo", Color.Indigo },
            { "Ivory", Color.Ivory },
            { "Khaki", Color.Khaki },
            { "Lavender", Color.Lavender },
            { "LavenderBlush", Color.LavenderBlush },
            { "LawnGreen", Color.LawnGreen },
            { "LemonChiffon", Color.LemonChiffon },
            { "LightBlue", Color.LightBlue },
            { "LightCoral", Color.LightCoral },
            { "LightCyan", Color.LightCyan },
            { "LightGoldenrodYellow", Color.LightGoldenrodYellow },
            { "LightGray", Color.LightGray },
            { "LightGreen", Color.LightGreen },
            { "LightPink", Color.LightPink },
            { "LightSalmon", Color.LightSalmon },
            { "LightSeaGreen", Color.LightSeaGreen },
            { "LightSkyBlue", Color.LightSkyBlue },
            { "LightSlateGray", Color.LightSlateGray },
            { "LightSteelBlue", Color.LightSteelBlue },
            { "LightYellow", Color.LightYellow },
            { "Lime", Color.Lime },
            { "LimeGreen", Color.LimeGreen },
            { "Linen", Color.Linen },
            { "Magenta", Color.Magenta },
            { "Maroon", Color.Maroon },
            { "MediumAquamarine", Color.MediumAquamarine },
            { "MediumBlue", Color.MediumBlue },
            { "MediumOrchid", Color.MediumOrchid },
            { "MediumPurple", Color.MediumPurple },
            { "MediumSeaGreen", Color.MediumSeaGreen },
            { "MediumSlateBlue", Color.MediumSlateBlue },
            { "MediumSpringGreen", Color.MediumSpringGreen },
            { "MediumTurquoise", Color.MediumTurquoise },
            { "MediumVioletRed", Color.MediumVioletRed },
            { "MidnightBlue", Color.MidnightBlue },
            { "MintCream", Color.MintCream },
            { "MistyRose", Color.MistyRose },
            { "Moccasin", Color.Moccasin },
            { "NavajoWhite", Color.NavajoWhite },
            { "Navy", Color.Navy },
            { "OldLace", Color.OldLace },
            { "Olive", Color.Olive },
            { "OliveDrab", Color.OliveDrab },
            { "Orange", Color.Orange },
            { "OrangeRed", Color.OrangeRed },
            { "Orchid", Color.Orchid },
            { "PaleGoldenrod", Color.PaleGoldenrod },
            { "PaleGreen", Color.PaleGreen },
            { "PaleTurquoise", Color.PaleTurquoise },
            { "PaleVioletRed", Color.PaleVioletRed },
            { "PapayaWhip", Color.PapayaWhip },
            { "PeachPuff", Color.PeachPuff },
            { "Peru", Color.Peru },
            { "Pink", Color.Pink },
            { "Plum", Color.Plum },
            { "PowderBlue", Color.PowderBlue },
            { "Purple", Color.Purple },
            { "Red", Color.Red },
            { "RosyBrown", Color.RosyBrown },
            { "RoyalBlue", Color.RoyalBlue },
            { "SaddleBrown", Color.SaddleBrown },
            { "Salmon", Color.Salmon },
            { "SandyBrown", Color.SandyBrown },
            { "SeaGreen", Color.SeaGreen },
            { "SeaShell", Color.SeaShell },
            { "Sienna", Color.Sienna },
            { "Silver", Color.Silver },
            { "SkyBlue", Color.SkyBlue },
            { "SlateBlue", Color.SlateBlue },
            { "SlateGray", Color.SlateGray },
            { "Snow", Color.Snow },
            { "SpringGreen", Color.SpringGreen },
            { "SteelBlue", Color.SteelBlue },
            { "Tan", Color.Tan },
            { "Teal", Color.Teal },
            { "Thistle", Color.Thistle },
            { "Tomato", Color.Tomato },
            { "Transparent", Color.Transparent },
            { "Turquoise", Color.Turquoise },
            { "Violet", Color.Violet },
            { "Wheat", Color.Wheat },
            { "White", Color.White },
            { "WhiteSmoke", Color.WhiteSmoke },
            { "Yellow", Color.Yellow },
            { "YellowGreen", Color.YellowGreen },
            { "MonoGameOrange", Color.MonoGameOrange }
        };

        /// <summary>
        /// Diccionario que mapea nombres de atlas de texturas a sus instancias TextureAtlas cargadas.
        /// <para>Dictionary mapping texture atlas names to their loaded TextureAtlas instances.</para>
        /// </summary>
        /// <remarks>
        /// Se llena durante la carga de escenas y se usa para renderizado de sprites.
        /// <para>This is populated during scene loading and used for sprite rendering.</para>
        /// </remarks>
        public static Dictionary<string, TextureAtlas> TextureAtlasDictionary = new Dictionary<string, TextureAtlas>();

        /// <summary>
        /// Obtiene el administrador del dispositivo gráfico actual.
        /// <para>Gets the current graphics device manager.</para>
        /// </summary>
        public static GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        /// <summary>
        /// Obtiene el content manager compartido.
        /// <para>Gets the shared content manager.</para>
        /// </summary>
        public static ContentManager ContentManager => YTBGlobalState.ContentManager;

        /// <summary>
        /// Actualiza el estado del administrador de escenas desde archivos YTB.
        /// <para>Updates the current scene manager state from YTB files.</para>
        /// </summary>
        /// <returns>Tarea de la operación. <para>Task for the operation.</para></returns>
        public async static Task UpdateStateOfSceneManager()
        {
            try
            {
                GameWontRun.Reset();

                var cm = ContentManager;

                var game = await ReadYTBFile.ReadYTBFiles(true);

                SceneManager sceneManager = new(GraphicsDeviceManager);

                foreach (var element in game.Item1.Scene)
                {
                    string entityFollowCameraName = string.Empty;
                    //Creo la escena y lleno sus propiedades
                    Scene scene = new(GraphicsDeviceManager);
                    scene.EventManager = EventManager.Instance;
                    scene.EntityManager = new EntityManager();
                    scene.SceneName = element.Name;

                    //Agrego entidades a la escena
                    foreach (YTBEntity en in element.Entities)
                    {
                        //Creo la entidad y lleno sus propiedades
                        Yotsuba entity = new Yotsuba(scene.EntityManager.YotsubaEntities.Count + 1);
                        entity.Name = en.Name;
                        //Agrego la entidad a su arreglo de entidades
                        scene.EntityManager.AddEntity(ref entity);

                        //Agrego componentes a las entidades
                        foreach (var component in en.Components)
                        {
                            if (EntityYTBXmlTemplate.GenerateNew().Components.Contains(component))
                                continue;
                            switch (component.ComponentName)
                            {
                                case nameof(TransformComponent):
                                    TransformComponent transformComponent = ConvertToTransform(component, element.Name, entity.Name);
                                    scene.EntityManager.AddTransformComponent(entity, transformComponent);
                                    break;
                                case nameof(SpriteComponent2D):
                                    SpriteComponent2D spriteComponent2D = ConvertToSprite(component, entity.Name, element.Name);
                                    scene.EntityManager.AddSpriteComponent(entity, spriteComponent2D);
                                    break;
                                case nameof(RigidBodyComponent2D):
                                    RigidBodyComponent2D rigidBodyComponent2D = ConvertTo2DRigibody(component, entity.Name, element.Name);
                                    scene.EntityManager.AddRigidbodyComponent(entity, rigidBodyComponent2D);
                                    break;
                                case nameof(ButtonComponent2D):
                                    ButtonComponent2D buttonComponent2D = ConvertTo2DButton(component, entity.Name, element.Name);
                                    scene.EntityManager.AddButtonComponent2D(entity, buttonComponent2D);
                                    break;
                                case nameof(AnimationComponent2D):
                                    AnimationComponent2D animationComponent2D = ConvertToAnimation(component, entity.Name, element.Name);
                                    scene.EntityManager.AddAnimationComponent(entity, animationComponent2D);
                                    break;
                                case nameof(ModelComponent3D):
                                    ModelComponent3D modelComponent3D = ConvertToModel3D(component, entity.Name, element.Name);
                                    if (modelComponent3D.Model != null)
                                    {
                                        scene.EntityManager.AddModelComponent3D(entity, modelComponent3D);
                                    }
                                    break;
                                case nameof(RigidBodyComponent3D):
                                    // 3D rigid body support is not yet implemented (Coming Soon)
                                    EngineUISystem.SendLog($"Warning: RigidBodyComponent3D not supported yet for entity {entity.Name}. Skipping component.");
                                    break;
                                case nameof(CameraComponent3D):
                                    CameraComponent3D cameraComponent = ConvertToCamera(scene.EntityManager,component, entity.Id, out string entityFollowName, element.Name, entity.Name);
                                    entityFollowCameraName = entityFollowName;
                                    scene.EntityManager.AddCameraComponent(cameraComponent, entity);
                                    scene.EntityManager.Camera = cameraComponent;
                                    break;
                                case nameof(InputComponent):
                                    InputComponent inputComponent = ConvertToInput(component, entity.Name, element.Name);
                                    scene.EntityManager.AddInputComponent(entity, inputComponent);
                                    break;
                                case nameof(ScriptComponent):
                                    if (component.Propiedades[0].Item1 == "Scripts")
                                    {
                                        string mainScript = component.Propiedades[0].Item2.Split("&;&")[0];

                                        string scriptType = mainScript.Split("&:&")[0];
                                        string path = mainScript.Split("&:&")[1];
                                        if (String.IsNullOrEmpty(path)) break;
                                    }
                                    ScriptComponent scriptComponent = ConvertToScript(component, entity);
                                    scene.EntityManager.AddScriptComponent(entity, scriptComponent);
                                    break;
                                case nameof(TileMapComponent2D):
                                    if (String.IsNullOrEmpty(component.Propiedades[0].Item2))
                                        break;
                                    scene.EntityManager.AddTileMapComponent(entity, TiledManager.GenerateTilemapComponent(component.Propiedades[0].Item2));
                                    break;
                                case nameof(FontComponent2D):
                                    FontComponent2D fontComponent2D = ConvertToFont(component, entity.Name, element.Name);
                                    scene.EntityManager.AddFontComponent2D(entity, fontComponent2D);
                                    break;
                                case nameof(ShaderComponent):
                                    ShaderComponent shaderComponent2D = ConvertToShader(component, entity.Name, element.Name);
                                    scene.EntityManager.AddShaderComponent2D(entity, shaderComponent2D);
                                    break;
                                default:
                                    _ = new GameWontRun(YTBErrors.ComponentUnknown, element.Name, entity.Name, component.ComponentName, "",
                                        $"Componente desconocido: '{component.ComponentName}'",
                                        "Verifique que el nombre del componente sea válido o que esté implementado en el engine");
                                    break;
                            }
                        }



                    }
                    //Agrego la escena al arreglo de escenas
                    sceneManager.Scenes.Add(scene);

					if (scene.EntityManager.Camera == null)
					{
						_ = new GameWontRun($"El juego necesita que la escena {scene.SceneName} tenga una camara para poder continuar.", YTBErrors.CameraNotFound);
						continue;
					}

					Yotsuba ent = scene.EntityManager.YotsubaEntities._ytb.FirstOrDefault(x => x.Name == entityFollowCameraName);
                    if (ent.Id > 0)
                    {
                        scene.EntityManager.Camera.EntityToFollow = ent.Id;
                    }
                    else
                    {
						_ = new GameWontRun($"El juego necesita que en la escena {scene.SceneName} la camara siga a una entidad. Por favor, selecciona una entidad para que la camara la siga.", YTBErrors.CameraNotFound);
					}

                    sceneManager.Scenes.Add(scene);
				}
                if (!String.IsNullOrEmpty(YTBGlobalState.LastSceneNameBeforeUpdate))
                {
                    sceneManager.CurrentScene = sceneManager.Scenes._ytb.Any(x => x.SceneName == YTBGlobalState.LastSceneNameBeforeUpdate) ? sceneManager.Scenes._ytb.FirstOrDefault(x => x.SceneName == YTBGlobalState.LastSceneNameBeforeUpdate) : sceneManager.Scenes[0];
                    AudioSystem.PauseAll();
                }
                else
                {

                    sceneManager.CurrentScene = sceneManager.Scenes._ytb.Any(x => x.SceneName == "Index") ? sceneManager.Scenes._ytb.FirstOrDefault(x => x.SceneName == "Index") : sceneManager.Scenes[0];
                    AudioSystem.PauseAll();

                }
//-:cnd:noEmit
#if YTB
                EventManager.Instance.Publish(new OnChangeEsceneManager(sceneManager));
#endif
//+:cnd:noEmit
                EngineUISystem.SendLog("Se ha actualizado el juego correctamente");
            } catch (Exception ex)
            {

                if (!GameWontRun.GameWontRunByException)
                {
                    try
                    {
                        _ = new GameWontRun("No se pudo actualizar el juego correctamente, revise el log para mas detalles.", YTBErrors.Unknown);
                        _ = new GameWontRun(ex, YTBErrors.Unknown);
                    }
                    catch
                    {
                        return;
                    }
                }
            }
        }




        /// <summary>
        /// Genera un administrador de escenas desde los datos YTB actuales.
        /// <para>Generates a scene manager from the current YTB data.</para>
        /// </summary>
        /// <param name="graphicsDeviceManager">Administrador de dispositivo gráfico. <para>Graphics device manager.</para></param>
        /// <returns>El administrador de escenas generado. <para>The generated scene manager.</para></returns>
        public static SceneManager GenerateSceneManager(GraphicsDeviceManager graphicsDeviceManager)
        {
            SceneManager sceneManager;
            try
            {
                GraphicsDeviceManager = graphicsDeviceManager;
                WriteYTBFile.RefactorYTBFile().GetAwaiter().GetResult();

                var game = ReadYTBFile.ReadYTBFiles(false).GetAwaiter().GetResult();

                sceneManager = new(graphicsDeviceManager);

                foreach (var element in game.Item1.Scene)
                {
                    string entityFollowCameraName = string.Empty;

                    //Creo la escena y lleno sus propiedades
                    Scene scene = new(graphicsDeviceManager);
                    scene.EventManager = EventManager.Instance;
                    scene.EntityManager = new EntityManager();
                    scene.SceneName = element.Name;

                    if(element.Entities == null)
                    {
                        _ = new GameWontRun($"Tu escena {element.Name} no tiene entidades. Si lo dejas asi, tu juego en produccion no funcionara.", YTBErrors.GameSceneWithoutEntities);
                        continue;
                    }
                    //Agrego entidades a la escena
                    foreach (YTBEntity en in element.Entities)
                    {
                        //Creo la entidad y lleno sus propiedades
                        Yotsuba entity = new Yotsuba(scene.EntityManager.YotsubaEntities.Count + 1);
                        entity.Name = en.Name;
                        //Agrego la entidad a su arreglo de entidades
                        scene.EntityManager.AddEntity(ref entity);

                        //Agrego componentes a las entidades
                        foreach (var component in en.Components)
                        {
                            if (EntityYTBXmlTemplate.GenerateNew().Components.Contains(component))
                                continue;
                            switch (component.ComponentName)
                            {
                                case nameof(TransformComponent):
                                    TransformComponent transformComponent = ConvertToTransform(component, element.Name, entity.Name);
                                    scene.EntityManager.AddTransformComponent(entity, transformComponent);
                                    break;
                                case nameof(SpriteComponent2D):
                                    SpriteComponent2D spriteComponent2D = ConvertToSprite(component, entity.Name, element.Name);
                                    scene.EntityManager.AddSpriteComponent(entity, spriteComponent2D);
                                    break;
                                case nameof(RigidBodyComponent2D):
                                    RigidBodyComponent2D rigidBodyComponent2D = ConvertTo2DRigibody(component, entity.Name, element.Name);
                                    scene.EntityManager.AddRigidbodyComponent(entity, rigidBodyComponent2D);
                                    break;
                                case nameof(ButtonComponent2D):
                                    ButtonComponent2D buttonComponent2D = ConvertTo2DButton(component, entity.Name, element.Name);
                                    scene.EntityManager.AddButtonComponent2D(entity, buttonComponent2D);
                                    break;
                                case nameof(AnimationComponent2D):
                                    AnimationComponent2D animationComponent2D = ConvertToAnimation(component, entity.Name, element.Name);
                                    scene.EntityManager.AddAnimationComponent(entity, animationComponent2D);
                                    break;
                                case nameof(ModelComponent3D):
                                    ModelComponent3D modelComponent3D = ConvertToModel3D(component, entity.Name, element.Name);
                                    if (modelComponent3D.Model != null)
                                    {
                                        scene.EntityManager.AddModelComponent3D(entity, modelComponent3D);
                                    }
                                    break;
                                case nameof(RigidBodyComponent3D):
                                    // 3D rigid body support is not yet implemented (Coming Soon)
                                    EngineUISystem.SendLog($"Warning: RigidBodyComponent3D not supported yet for entity {entity.Name}. Skipping component.");
                                    break;
                                case nameof(CameraComponent3D):
                                    CameraComponent3D cameraComponent = ConvertToCamera(scene.EntityManager, component, entity.Id, out string entityFollowName, element.Name, entity.Name);
                                    entityFollowCameraName = entityFollowName;
                                    scene.EntityManager.AddCameraComponent(cameraComponent, entity);
                                    scene.EntityManager.Camera = cameraComponent;
                                    break;
                                case nameof(InputComponent):
                                    InputComponent inputComponent = ConvertToInput(component, entity.Name, element.Name);
                                    scene.EntityManager.AddInputComponent(entity, inputComponent);
                                    break;
                                case nameof(ScriptComponent):
                                    if (component.Propiedades[0].Item1 == "Scripts")
                                    {
                                        string mainScript = component.Propiedades[0].Item2.Split("&;&")[0];

                                        string scriptType = mainScript.Split("&:&")[0];
                                        string path = mainScript.Split("&:&")[1];
                                        if (String.IsNullOrEmpty(path)) break;
                                    }
                                    ScriptComponent scriptComponent = ConvertToScript(component, entity);
                                    scene.EntityManager.AddScriptComponent(entity, scriptComponent);
                                    break;
                                case nameof(TileMapComponent2D):
                                    if (String.IsNullOrEmpty(component.Propiedades[0].Item2))
                                        break;
                                    try
                                    {
                                        scene.EntityManager.AddTileMapComponent(entity, TiledManager.GenerateTilemapComponent(component.Propiedades[0].Item2));
                                    }
                                    catch (Exception ex)
                                    {
                                        _ = new GameWontRun(YTBErrors.TileMapParseFailed, element.Name, entity.Name, nameof(TileMapComponent2D), "", $"Error al parsear TileMapComponent2D: {ex.Message}", "Revise que el archivo TMX sea válido y que la ruta sea correcta");
                                    }
                                    break;
                                case nameof(FontComponent2D):
                                    FontComponent2D fontComponent2D = ConvertToFont(component, entity.Name, element.Name);
                                    scene.EntityManager.AddFontComponent2D(entity, fontComponent2D);
                                    break;
                                case nameof(ShaderComponent):
                                    ShaderComponent shaderComponent2D = ConvertToShader(component, entity.Name, element.Name);
                                    scene.EntityManager.AddShaderComponent2D(entity, shaderComponent2D);
                                    break;
                                default:
                                    _ = new GameWontRun(YTBErrors.ComponentUnknown, element.Name, entity.Name, component.ComponentName, "",
                                        $"Componente desconocido: '{component.ComponentName}'",
                                        "Verifique que el nombre del componente sea válido o que esté implementado en el engine");
                                    break;
                            }
                        }


                    }

                    if(scene.EntityManager.Camera == null)
                    {
						_ = new GameWontRun($"El juego necesita que la escena {scene.SceneName} tenga una camara para poder continuar.", YTBErrors.CameraNotFound);
						continue;
					}

					//Agrego la escena al arreglo de escenas
					sceneManager.Scenes.Add(scene);
                    Yotsuba ent = scene.EntityManager.YotsubaEntities._ytb.FirstOrDefault(x => x.Name == entityFollowCameraName);
                    if (ent.Name is not null)
                    {
                        scene.EntityManager.Camera.EntityToFollow = ent.Id;
                    }
                    else
                    {
                        _ = new GameWontRun($"El juego necesita que en la escena {scene.SceneName} la camara siga a una entidad. Por favor, selecciona una entidad para que la camara la siga.", YTBErrors.CameraFollowNothing);
                    }
                }

                if (sceneManager.Scenes.Count > 0)
                {
                    sceneManager.CurrentScene = sceneManager.Scenes._ytb.Any(x => x.SceneName == "Index") ? sceneManager.Scenes._ytb.FirstOrDefault(x => x.SceneName == "Index") : sceneManager.Scenes[0];


                }
                else
                {
                    _ = new GameWontRun($"El juego necesita tener escenas para poder funcionar, por favor, considera crear una.", YTBErrors.GameWithoutScenes);
                }
                
                return sceneManager;
            }
            catch (Exception ex)
            {
                sceneManager = new SceneManager(graphicsDeviceManager) { CurrentScene = new(graphicsDeviceManager), Scenes = new YTB<Scene>() };
                if (!GameWontRun.GameWontRunByException)
                {
                    try
                    {
                        _ = new GameWontRun("No se pudo generar el SceneManager correctamente, revise el log para mas detalles.", YTBErrors.Unknown);
                        _ = new GameWontRun(ex, YTBErrors.Unknown);
                    }
                    catch
                    {
                        return sceneManager;
                    }
                }
                
                return sceneManager;
            }
        }







        #region Metodos que procesaran toda la logica para convertir las propiedades del json en propiedades de un componente


        /// <summary>
        /// Metodo que crea un componente de script a partir de un componente de archivo YTB
        /// </summary>
        /// <param name="component"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <summary>
        /// Converts YTB component data into a script component.
        /// Convierte datos de componente YTB en un componente de script.
        /// </summary>
        /// <param name="component">Component data. Datos del componente.</param>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        /// <returns>Script component. Componente de script.</returns>
        private static ScriptComponent ConvertToScript(YTBComponents component, Yotsuba entity)
        {
            ScriptComponent scriptComponent = new ScriptComponent();

            
            try
            {
                foreach (var prop in component.Propiedades)
                {
                    switch (prop.Item1)
                    {
                        case "Scripts":
                            string scripts = prop.Item2;
                            Dictionary<ScriptComponentType, string> scriptLanguages = new Dictionary<ScriptComponentType, string>(3);

                            var scriptPath = prop.Item2.Split("&;&")[0];
                        
                                string type = scriptPath.Split("&:&")[0];
                                string path = scriptPath.Split("&:&")[1];
                                if (Enum.TryParse<ScriptComponentType>(type, true, out var result))
                                {
                                    scriptLanguages.Add(result, path);
                                }
                        

                            scriptComponent.ScriptLanguaje = scriptLanguages;
                            break;
                    }
                }

                foreach(ref BaseScript script in scriptComponent.Scripts.AsSpan())
                {
                    script.EntityId = entity.Id;
                }
            }
            catch (Exception ex)
            {
                _ = new GameWontRun(YTBErrors.ScriptParseFailed, "", entity.Name, nameof(ScriptComponent), "", $"Error al parsear ScriptComponent: {ex.Message}", "Revise que las propiedades del script tengan el formato correcto (tipo&:&ruta separados por &;&)");
            }

            return scriptComponent;
        }


        /// <summary>
        /// Método que convierte el formato json del componente de transformación a formato entendible del juego (Un TransformComponent).
        /// </summary>
        /// <param name="component"></param>
        /// <returns>Retorna un TransformComponent</returns>
        /// <summary>
        /// Converts YTB component data into a transform component.
        /// Convierte datos de componente YTB en un componente de transformación.
        /// </summary>
        /// <param name="component">Component data. Datos del componente.</param>
        /// <returns>Transform component. Componente de transformación.</returns>
        private static TransformComponent ConvertToTransform(YTBComponents component, string sceneName = "", string entityName = "")
        {
            TransformComponent transform = new();
            try
            {
            foreach(var prop in component.Propiedades)
            {
                switch (prop.Item1)
                {

                    case nameof(transform.Scale):
                        if (float.TryParse(prop.Item2, out float scale))
                            transform.Scale = scale;
                        else
                            _ = new GameWontRun(YTBErrors.TransformParseFailed, sceneName, entityName, nameof(TransformComponent), nameof(transform.Scale), $"No se pudo parsear Scale: '{prop.Item2}'", "Asegúrese de que Scale sea un número decimal válido (ej: 1.0)");
                        break;

                    case nameof(transform.Rotation):
                        if(float.TryParse(prop.Item2, out float rotation))
                            transform.Rotation = rotation;
                        else
                            _ = new GameWontRun(YTBErrors.TransformParseFailed, sceneName, entityName, nameof(TransformComponent), nameof(transform.Rotation), $"No se pudo parsear Rotation: '{prop.Item2}'", "Asegúrese de que Rotation sea un número decimal válido (ej: 0.0)");
                        break;

                    case nameof(transform.Position):
                        string[] dimen = prop.Item2.Split(",");
                        if(dimen.Length >= 3 && float.TryParse(dimen[0], out float Px) && float.TryParse(dimen[1], out float Py) && float.TryParse(dimen[2], out float Pz))
                            transform.Position = new Vector3(Px, Py, Pz);
                        else
                            _ = new GameWontRun(YTBErrors.TransformParseFailed, sceneName, entityName, nameof(TransformComponent), nameof(transform.Position), $"No se pudo parsear Position: '{prop.Item2}'", "Asegúrese de que Position tenga el formato 'X,Y,Z' con números decimales (ej: 0,0,0)");
                        break;

                    case nameof(transform.Size):
                        string[] sizes = prop.Item2.Split(",");
                        if (sizes.Length >= 3 && float.TryParse(sizes[0], out float Sx) && float.TryParse(sizes[1], out float Sy) && float.TryParse(sizes[2], out float Sz))
                            transform.Size = new Vector3(Sx, Sy, Sz);
                        else
                            _ = new GameWontRun(YTBErrors.TransformParseFailed, sceneName, entityName, nameof(TransformComponent), nameof(transform.Size), $"No se pudo parsear Size: '{prop.Item2}'", "Asegúrese de que Size tenga el formato 'X,Y,Z' con números decimales (ej: 1,1,1)");
                        break;

                    case nameof(transform.LayerDepth):
                        if(int.TryParse(prop.Item2, out int depth))
                            transform.LayerDepth = depth;
                        else
                            _ = new GameWontRun(YTBErrors.TransformParseFailed, sceneName, entityName, nameof(TransformComponent), nameof(transform.LayerDepth), $"No se pudo parsear LayerDepth: '{prop.Item2}'", "Asegúrese de que LayerDepth sea un número entero válido (ej: 0)");
                        break;

                    case nameof(transform.SpriteEffects):
                        if (Enum.TryParse(prop.Item2, out SpriteEffects effect))
                            transform.SpriteEffects = effect;
                        else
                            _ = new GameWontRun(YTBErrors.TransformParseFailed, sceneName, entityName, nameof(TransformComponent), nameof(transform.SpriteEffects), $"No se pudo parsear SpriteEffects: '{prop.Item2}'", "Asegúrese de que SpriteEffects sea un valor válido: None, FlipHorizontally, FlipVertically");
                        break;

                    case nameof(transform.Color):
                        // AOT-compatible color parsing using static dictionary instead of reflection
                        if (NamedColors.TryGetValue(prop.Item2, out Color namedColor))
                        {
                            transform.Color = namedColor;
                        }
                        else
                        {
                            transform.Color = Color.White;
                            _ = new GameWontRun(YTBErrors.TransformParseFailed, sceneName, entityName, nameof(TransformComponent), nameof(transform.Color), $"Color no reconocido: '{prop.Item2}', usando White por defecto", "Use un nombre de color válido de XNA (ej: Red, Blue, White, Black)");
                        }   
                        break;
                }
            }
            }
            catch (Exception ex)
            {
                _ = new GameWontRun(YTBErrors.TransformParseFailed, sceneName, entityName, nameof(TransformComponent), "", $"Error al parsear TransformComponent: {ex.Message}", "Revise que todas las propiedades del TransformComponent sean válidas");
            }

            return transform;
        }

        /// <summary>
        /// Método que convierte el formato json del componente de sprite a formato entendible del juego (Un SpriteComponent).
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        /// <summary>
        /// Converts YTB component data into a sprite component.
        /// Convierte datos de componente YTB en un componente de sprite.
        /// </summary>
        /// <param name="component">Component data. Datos del componente.</param>
        /// <param name="entityName">Entity name. Nombre de la entidad.</param>
        /// <returns>Sprite component. Componente de sprite.</returns>
        private static SpriteComponent2D ConvertToSprite(YTBComponents component, string entityName, string sceneName = "")
        {
            try
            {
                SpriteComponent2D spriteComponent = new SpriteComponent2D();
                string TextureAtlasPath = component.Propiedades.FirstOrDefault(x => x.Item1 == "TextureAtlasPath").Item2;

                if (string.IsNullOrWhiteSpace(TextureAtlasPath))
                {
                    _ = new GameWontRun(YTBErrors.SpriteParseFailed, sceneName, entityName, nameof(SpriteComponent2D), "TextureAtlasPath", "TextureAtlasPath está vacío o es nulo", "Asigne una ruta válida al atlas de texturas en el componente SpriteComponent2D");
                    return spriteComponent;
                }

                if (!TextureAtlasDictionary.ContainsKey(TextureAtlasPath))
                {
                    TextureAtlas textureAtlas;
                    textureAtlas = YotsubaGraphicsManager.TextureAtlasGenerator(ContentManager, TextureAtlasPath);
                    TextureAtlasDictionary.Add(TextureAtlasPath, textureAtlas);
                }

                if (TextureAtlasDictionary.TryGetValue(TextureAtlasPath, out TextureAtlas TexureAtlas))
                {

                    string spriteName = component.Propiedades.FirstOrDefault(x => x.Item1 == "SpriteName").Item2;
                    string[] SourceRectangle = component.Propiedades.FirstOrDefault(x => x.Item1 == "SourceRectangle").Item2.Split(",");

                    bool.TryParse(component.Propiedades.FirstOrDefault(x => x.Item1 == nameof(spriteComponent.IsVisible)).Item2, out bool IsVisible);

                    TextureRegion region = TexureAtlas.GetRegion(spriteName);
                    TextureRegion finalRegion = new TextureRegion(region.Texture, region.SourceRectangle);
                    if (int.TryParse(SourceRectangle[0], out int x) && int.TryParse(SourceRectangle[1], out int y) && int.TryParse(SourceRectangle[2], out int w) && int.TryParse(SourceRectangle[3], out int h))
                    {
                        Rectangle zone = new Rectangle(x, y, w, h);
                        finalRegion = new TextureRegion(region.Texture, zone);
                    }

                    spriteComponent = YotsubaGraphicsManager.SpriteGenerator(region);
                }

                if (bool.TryParse(component.Propiedades.FirstOrDefault(x => x.Item1 == "IsVisible").Item2, out bool isVisible))
                {
                    spriteComponent.IsVisible = isVisible;
                }

                string? is2_5D = component.Propiedades.FirstOrDefault(x => x.Item1 == "2.5D").Item2;
                if (!String.IsNullOrEmpty(is2_5D))
                {
                    if (bool.TryParse(is2_5D, out bool dimencion2_5))
                    {
                        spriteComponent.Is2_5D = dimencion2_5;
                    }
                }
                else
                {
                    spriteComponent.Is2_5D = false;
                }

                return spriteComponent;
            }
            catch (Exception ex)
            {
                _ = new GameWontRun(YTBErrors.SpriteParseFailed, sceneName, entityName, nameof(SpriteComponent2D), "", $"Error al parsear SpriteComponent2D: {ex.Message}", "Revise que todas las propiedades del sprite (TextureAtlasPath, SpriteName, SourceRectangle) sean válidas");
                return new SpriteComponent2D();
			}
        }

        /// <summary>
        /// Método que convierte el formato json del componente de Rigibody2D a formato entendible del juego (Un RigibodyComponent).
        /// </summary>
        /// <param name="component"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <summary>
        /// Converts YTB component data into a 2D rigid body component.
        /// Convierte datos de componente YTB en un componente de cuerpo rígido 2D.
        /// </summary>
        /// <param name="component">Component data. Datos del componente.</param>
        /// <param name="name">Entity name. Nombre de la entidad.</param>
        /// <returns>Rigid body component. Componente de cuerpo rígido.</returns>
        private static RigidBodyComponent2D ConvertTo2DRigibody(YTBComponents component, string name, string sceneName = "")
        {
            RigidBodyComponent2D rigidBodyComponent2D = new RigidBodyComponent2D();
            try
            {
            string OffsetCollision = component.Propiedades.FirstOrDefault(x => x.Item1 == "OffSetCollision").Item2;
            string Velocity = component.Propiedades.FirstOrDefault(x => x.Item1 == "Velocity").Item2;
            string GameType = component.Propiedades.FirstOrDefault(x => x.Item1 == "GameType").Item2;
            string Mass = component.Propiedades.FirstOrDefault(x => x.Item1 == "Mass").Item2;

            if(int.TryParse(OffsetCollision.Split(",")[0], out int Ox) && int.TryParse(OffsetCollision.Split(",")[1], out int Oy))
            {
                rigidBodyComponent2D.OffSetCollision = new Vector2(Ox, Oy);
            }
            else
            {
                _ = new GameWontRun(YTBErrors.RigidBody2DParseFailed, sceneName, name, nameof(RigidBodyComponent2D), "OffSetCollision", $"No se pudo parsear OffSetCollision: '{OffsetCollision}'", "Asegúrese de que OffSetCollision tenga el formato 'X,Y' con números enteros (ej: 0,0)");
            }

            if (float.TryParse(Velocity.Split(",")[0], out float Vx) && float.TryParse(Velocity.Split(",")[1], out float Xx))
            {
                rigidBodyComponent2D.Velocity = new Vector3(Vx, Xx, 0);
            }
            else
            {
                _ = new GameWontRun(YTBErrors.RigidBody2DParseFailed, sceneName, name, nameof(RigidBodyComponent2D), "Velocity", $"No se pudo parsear Velocity: '{Velocity}'", "Asegúrese de que Velocity tenga el formato 'X,Y' con números decimales (ej: 0,0)");
            }

            if (Enum.TryParse(GameType, out GameType gameType))
            {
                rigidBodyComponent2D.GameType = gameType;
            }
            else
            {
                _ = new GameWontRun(YTBErrors.RigidBody2DParseFailed, sceneName, name, nameof(RigidBodyComponent2D), "GameType", $"No se pudo parsear GameType: '{GameType}'", "Asegúrese de que GameType sea un valor válido del enum GameType");
            }

            if(Enum.TryParse(Mass, out MassLevel mass))
            {
                rigidBodyComponent2D.Mass = mass;
            }
            else
            {
                _ = new GameWontRun(YTBErrors.RigidBody2DParseFailed, sceneName, name, nameof(RigidBodyComponent2D), "Mass", $"No se pudo parsear Mass: '{Mass}'", "Asegúrese de que Mass sea un valor válido del enum MassLevel");
            }
            }
            catch (Exception ex)
            {
                _ = new GameWontRun(YTBErrors.RigidBody2DParseFailed, sceneName, name, nameof(RigidBodyComponent2D), "", $"Error al parsear RigidBodyComponent2D: {ex.Message}", "Revise que todas las propiedades del RigidBody (OffSetCollision, Velocity, GameType, Mass) sean válidas");
            }
            
            return rigidBodyComponent2D;
        }


        /// <summary>
        /// Crea un nuevo ButtonComponent2D a partir de un componente YTBComponents dado.
        /// </summary>
        /// <param name="component">El componente YTBComponents que contiene las propiedades del botón.</param>
        /// <param name="name">El nombre de la entidad a la que pertenece el botón.</param>
        /// <returns>Retorna un ButtonComponent2D configurado según las propiedades del componente YTBComponents.</returns>
        /// <summary>
        /// Converts YTB component data into a 2D button component.
        /// Convierte datos de componente YTB en un componente de botón 2D.
        /// </summary>
        /// <param name="component">Component data. Datos del componente.</param>
        /// <param name="name">Entity name. Nombre de la entidad.</param>
        /// <returns>Button component. Componente de botón.</returns>
        private static ButtonComponent2D ConvertTo2DButton(YTBComponents component, string name, string sceneName = "")
        {
            ButtonComponent2D buttonComponent2D = new ButtonComponent2D();

            try
            {
            string EffectiveArea = component.Propiedades.FirstOrDefault(x => x.Item1 == "EffectiveArea").Item2;
            if (int.TryParse(EffectiveArea.Split(",")[0], out int x) && int.TryParse(EffectiveArea.Split(",")[1], out int y) &&
                int.TryParse(EffectiveArea.Split(",")[2], out int w) && int.TryParse(EffectiveArea.Split(",")[3], out int h))
            {
                buttonComponent2D.EffectiveArea = new Rectangle(x, y, w, h);
            }
            else
            {
                _ = new GameWontRun(YTBErrors.ButtonParseFailed, sceneName, name, nameof(ButtonComponent2D), "EffectiveArea", $"No se pudo parsear EffectiveArea: '{EffectiveArea}'", "Asegúrese de que EffectiveArea tenga el formato 'X,Y,Width,Height' con números enteros (ej: 0,0,100,50)");
            }

            bool.TryParse(component.Propiedades.FirstOrDefault(x => x.Item1 == nameof(buttonComponent2D.IsActive)).Item2, out bool IsActive);

            buttonComponent2D.IsActive = IsActive;
            }
            catch (Exception ex)
            {
                _ = new GameWontRun(YTBErrors.ButtonParseFailed, sceneName, name, nameof(ButtonComponent2D), "", $"Error al parsear ButtonComponent2D: {ex.Message}", "Revise que todas las propiedades del botón (EffectiveArea, IsActive) sean válidas");
            }

            return buttonComponent2D;
        }

        /// <summary>
        /// Convierte el componente YTB y el nombre especificados en una nueva instancia de CameraComponent.
        /// </summary>
        /// <param name="component">El componente YTB que se convertiría en una representación de cámara.</param>
        /// <param name="name">El nombre que se asignará al CameraComponent resultante.</param>
        /// <returns>Una instancia de CameraComponent que representa el componente YTB y el nombre especificados.</returns>
        /// <exception cref="NotImplementedException">Se lanza en todos los casos, ya que este método aún no está implementado.</exception>
        /// <summary>
        /// Converts YTB component data into a camera component.
        /// Convierte datos de componente YTB en un componente de cámara.
        /// </summary>
        /// <param name="component">Component data. Datos del componente.</param>
        /// <param name="entity">Entity identifier. Identificador de la entidad.</param>
        /// <returns>Camera component. Componente de cámara.</returns>
        private static CameraComponent3D ConvertToCamera(EntityManager entityManager, YTBComponents component, int entity, out string entityFollowName, string sceneName = "", string entityName = "")
        {
            entityFollowName = string.Empty;
            try
            {
            // Obtener la propiedad como string y dividirla
            string[] initialPositionStrs = component.Propiedades.FirstOrDefault(x => x.Item1 == "InitialPosition").Item2.Split(",");

            if (initialPositionStrs.Length < 3 ||
                !float.TryParse(initialPositionStrs[0], out float x) ||
                !float.TryParse(initialPositionStrs[1], out float y) ||
                !float.TryParse(initialPositionStrs[2], out float z))
            {
                _ = new GameWontRun(YTBErrors.CameraParseFailed, sceneName, entityName, nameof(CameraComponent3D), "InitialPosition", $"No se pudo parsear InitialPosition: '{string.Join(",", initialPositionStrs)}'", "Asegúrese de que InitialPosition tenga el formato 'X,Y,Z' con números decimales (ej: 0,0,0)");
                x = 0; y = 0; z = 0;
            }

            if (!float.TryParse(component.Propiedades.FirstOrDefault(p => p.Item1 == "AngleView").Item2, out float angleView))
            {
                _ = new GameWontRun(YTBErrors.CameraParseFailed, sceneName, entityName, nameof(CameraComponent3D), "AngleView", $"No se pudo parsear AngleView: '{component.Propiedades.FirstOrDefault(p => p.Item1 == "AngleView").Item2}'", "Asegúrese de que AngleView sea un número decimal válido (ej: 45.0)");
                angleView = 45f;
            }

            if (!float.TryParse(component.Propiedades.FirstOrDefault(p => p.Item1 == "NearRender").Item2, out float near))
            {
                _ = new GameWontRun(YTBErrors.CameraParseFailed, sceneName, entityName, nameof(CameraComponent3D), "NearRender", $"No se pudo parsear NearRender: '{component.Propiedades.FirstOrDefault(p => p.Item1 == "NearRender").Item2}'", "Asegúrese de que NearRender sea un número decimal válido (ej: 0.1)");
                near = 0.1f;
            }

            if (!float.TryParse(component.Propiedades.FirstOrDefault(p => p.Item1 == "FarRender").Item2, out float far))
            {
                _ = new GameWontRun(YTBErrors.CameraParseFailed, sceneName, entityName, nameof(CameraComponent3D), "FarRender", $"No se pudo parsear FarRender: '{component.Propiedades.FirstOrDefault(p => p.Item1 == "FarRender").Item2}'", "Asegúrese de que FarRender sea un número decimal válido (ej: 1000.0)");
                far = 1000f;
            }

            CameraComponent3D cameraComponent = new CameraComponent3D(entityManager, new Vector3(x, y, z), angleView, near, far);

            // Parsear OffsetCamera si existe
            string offsetStr = component.Propiedades.FirstOrDefault(x => x.Item1 == "OffsetCamera")?.Item2 ?? "0,50,-100";
            string[] offsetParts = offsetStr.Split(",");
            if (offsetParts.Length >= 3 &&
                float.TryParse(offsetParts[0], out float ox) &&
                float.TryParse(offsetParts[1], out float oy) &&
                float.TryParse(offsetParts[2], out float oz))
            {
                cameraComponent.OffsetCamera = new Vector3(ox, oy, oz);
            }
            // Si el parsing falla, mantiene el valor por defecto de OffsetCamera (0, 50, -100)

            entityFollowName = component.Propiedades.FirstOrDefault(x => x.Item1 == "EntityName").Item2;
            return cameraComponent;
            }
            catch (Exception ex)
            {
                _ = new GameWontRun(YTBErrors.CameraParseFailed, sceneName, entityName, nameof(CameraComponent3D), "", $"Error al parsear CameraComponent3D: {ex.Message}", "Revise que todas las propiedades de la cámara (InitialPosition, AngleView, NearRender, FarRender, EntityName) sean válidas");
                return new CameraComponent3D(entityManager, Vector3.Zero, 45f, 0.1f, 1000f);
            }
        }

        /// <summary>
        /// Convierte el componente YTB especificado en una instancia de AnimationComponent2D usando el nombre proporcionado.
        /// </summary>
        /// <param name="component">El componente YTB que se convertirá en una animación. No debe ser nulo.</param>
        /// <param name="name">El nombre que se asignará al componente de animación resultante. No puede ser nulo ni estar vacío.</param>
        /// <returns>Una instancia de AnimationComponent2D que representa el componente convertido con el nombre especificado.</returns>
        /// <exception cref="NotImplementedException">Se lanza en todos los casos, ya que este método aún no está implementado.</exception>
        /// <summary>
        /// Converts YTB component data into an animation component.
        /// Convierte datos de componente YTB en un componente de animación.
        /// </summary>
        /// <param name="component">Component data. Datos del componente.</param>
        /// <param name="name">Entity name. Nombre de la entidad.</param>
        /// <returns>Animation component. Componente de animación.</returns>
        private static AnimationComponent2D ConvertToAnimation(YTBComponents component, string name, string sceneName = "")
        {
            AnimationComponent2D animationComponent2D = new AnimationComponent2D();

            try
            {
            string TextureAtlasPath = component.Propiedades.FirstOrDefault(x => x.Item1 == "TextureAtlasPath").Item2;

            TextureAtlas atlas;
            if (TextureAtlasDictionary.ContainsKey(TextureAtlasPath)) atlas = TextureAtlasDictionary[TextureAtlasPath];
            else
            {
                atlas = YotsubaGraphicsManager.TextureAtlasGenerator(ContentManager, TextureAtlasPath);
                TextureAtlasDictionary.Add(TextureAtlasPath, atlas);
            }

            foreach (var prop in component.Propiedades)
            {
                switch (prop.Item1)
                {
                    case "AnimationBindings":
                        string bindings = prop.Item2;
                        foreach (string binding in bindings.Split(","))
                        {
                            string animationType = binding.Split(":")[0];
                            string animationName = binding.Split(":")[1];

                            Animation animation = atlas.GetAnimation(animationName);
                            if (Enum.TryParse(animationType, out AnimationType animType))
                            {
                                animationComponent2D.AddAnimation(animType, animation);
                            }
                            else
                            {
                                _ = new GameWontRun(YTBErrors.AnimationParseFailed, sceneName, name, nameof(AnimationComponent2D), "AnimationBindings", $"AnimationType no válido: '{animationType}'", "Asegúrese de que AnimationType sea un valor válido del enum AnimationType");
                            }
                        }
                        break;
                }
            }

            string CurrentAnimationType = component.Propiedades.FirstOrDefault(x => x.Item1 == "CurrentAnimationType").Item2;

            if (Enum.TryParse(CurrentAnimationType, out AnimationType currentAnimType))
            {
                animationComponent2D.CurrentAnimationType = (currentAnimType, animationComponent2D.GetAnimation(currentAnimType));
            }
            else
            {
                _ = new GameWontRun(YTBErrors.AnimationParseFailed, sceneName, name, nameof(AnimationComponent2D), "CurrentAnimationType", $"CurrentAnimationType no válido: '{CurrentAnimationType}'", "Asegúrese de que CurrentAnimationType sea un valor válido del enum AnimationType");
            }
            }
            catch (Exception ex)
            {
                _ = new GameWontRun(YTBErrors.AnimationParseFailed, sceneName, name, nameof(AnimationComponent2D), "", $"Error al parsear AnimationComponent2D: {ex.Message}", "Revise que todas las propiedades de la animación (TextureAtlasPath, AnimationBindings, CurrentAnimationType) sean válidas");
            }

            return animationComponent2D;
        }

        /// <summary>
        /// Convierte el componente YTB especificado en una instancia de InputComponent.
        /// Parsea los flags de input, mappings de teclado, gamepad y mouse desde el formato YTB.
        /// </summary>
        /// <param name="component">El componente YTB que contiene las propiedades de input.</param>
        /// <param name="name">El nombre de la entidad para mensajes de error.</param>
        /// <returns>Una instancia de InputComponent configurada con los mappings especificados.</returns>
        /// <exception cref="Exception">Se lanza si alguna propiedad no es válida o el parseo falla.</exception>
        /// <summary>
        /// Converts YTB component data into an input component.
        /// Convierte datos de componente YTB en un componente de entrada.
        /// </summary>
        /// <param name="component">Component data. Datos del componente.</param>
        /// <param name="name">Entity name. Nombre de la entidad.</param>
        /// <returns>Input component. Componente de entrada.</returns>
        private static InputComponent ConvertToInput(YTBComponents component, string name, string sceneName = "")
        {
            InputComponent inputComponent = new InputComponent();

            try
            {
            foreach (var prop in component.Propiedades)
            {
                switch (prop.Item1)
                {
                    case "InputsInUse":
                        if (!string.IsNullOrWhiteSpace(prop.Item2))
                        {
                            string[] flags = prop.Item2.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            foreach (string flag in flags)
                            {
                                string trimmedFlag = flag.Trim();
                                if (Enum.TryParse<InputInUse>(trimmedFlag, true, out var inputFlag))
                                {
                                    inputComponent.AddInput(inputFlag);
                                }
                                else
                                {
                                    _ = new GameWontRun(YTBErrors.InputParseFailed, sceneName, name, nameof(InputComponent), "InputsInUse", $"Flag de input inválido: '{trimmedFlag}'", "Asegúrese de que cada flag de InputsInUse sea un valor válido del enum InputInUse");
                                }
                            }
                        }
                        break;

                    case "GamePadIndex":
                        if (!string.IsNullOrWhiteSpace(prop.Item2))
                        {
                            if (Enum.TryParse<PlayerIndex>(prop.Item2, true, out var playerIndex))
                            {
                                inputComponent.GamePadIndex = playerIndex;
                            }
                            else
                            {
                                _ = new GameWontRun(YTBErrors.InputParseFailed, sceneName, name, nameof(InputComponent), "GamePadIndex", $"GamePadIndex inválido: '{prop.Item2}'", "Asegúrese de que GamePadIndex sea un valor válido: One, Two, Three, Four");
                            }
                        }
                        break;

                    case "KeyboardMappings":
                        if (!string.IsNullOrWhiteSpace(prop.Item2))
                        {
                            // Formato esperado: "Action:Key,Action:Key" (ej: "MoveUp:W,MoveDown:S")
                            string[] mappings = prop.Item2.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            foreach (string mapping in mappings)
                            {
                                string trimmedMapping = mapping.Trim();
                                if (string.IsNullOrWhiteSpace(trimmedMapping))
                                    continue;

                                string[] parts = trimmedMapping.Split(':', StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length != 2)
                                {
                                    _ = new GameWontRun(YTBErrors.InputParseFailed, sceneName, name, nameof(InputComponent), "KeyboardMappings", $"Formato inválido en KeyboardMappings: '{trimmedMapping}'. Formato esperado: 'Action:Key'", "Cada mapping debe tener el formato 'Accion:Tecla' separados por coma (ej: MoveUp:W,MoveDown:S)");
                                    continue;
                                }

                                string actionStr = parts[0].Trim();
                                string keyStr = parts[1].Trim();

                                if (!Enum.TryParse<ActionEntityInput>(actionStr, true, out var action))
                                {
                                    _ = new GameWontRun(YTBErrors.InputParseFailed, sceneName, name, nameof(InputComponent), "KeyboardMappings", $"Acción inválida en KeyboardMappings: '{actionStr}'", "Asegúrese de que la acción sea un valor válido del enum ActionEntityInput");
                                    continue;
                                }

                                if (!Enum.TryParse<Keys>(keyStr, true, out var key))
                                {
                                    _ = new GameWontRun(YTBErrors.InputParseFailed, sceneName, name, nameof(InputComponent), "KeyboardMappings", $"Tecla inválida en KeyboardMappings: '{keyStr}'", "Asegúrese de que la tecla sea un valor válido del enum Keys (ej: W, A, S, D, Space)");
                                    continue;
                                }

                                inputComponent.KeyBoard[action] = key;
                            }
                        }
                        break;

                    case "MouseMappings":
                        if (!string.IsNullOrWhiteSpace(prop.Item2))
                        {
                            // Formato esperado: "Action:Button,Action:Button" (ej: "Attack:Left,Dash:Right")
                            string[] mappings = prop.Item2.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            foreach (string mapping in mappings)
                            {
                                string trimmedMapping = mapping.Trim();
                                if (string.IsNullOrWhiteSpace(trimmedMapping))
                                    continue;

                                string[] parts = trimmedMapping.Split(':', StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length != 2)
                                {
                                    _ = new GameWontRun(YTBErrors.InputParseFailed, sceneName, name, nameof(InputComponent), "MouseMappings", $"Formato inválido en MouseMappings: '{trimmedMapping}'. Formato esperado: 'Action:Button'", "Cada mapping debe tener el formato 'Accion:Boton' separados por coma (ej: Attack:Left,Dash:Right)");
                                    continue;
                                }

                                string actionStr = parts[0].Trim();
                                string buttonStr = parts[1].Trim();

                                if (!Enum.TryParse<ActionEntityInput>(actionStr, true, out var action))
                                {
                                    _ = new GameWontRun(YTBErrors.InputParseFailed, sceneName, name, nameof(InputComponent), "MouseMappings", $"Acción inválida en MouseMappings: '{actionStr}'", "Asegúrese de que la acción sea un valor válido del enum ActionEntityInput");
                                    continue;
                                }

                                if (!Enum.TryParse<MouseButton>(buttonStr, true, out var button))
                                {
                                    _ = new GameWontRun(YTBErrors.InputParseFailed, sceneName, name, nameof(InputComponent), "MouseMappings", $"Botón de mouse inválido en MouseMappings: '{buttonStr}'", "Asegúrese de que el botón sea un valor válido del enum MouseButton (ej: Left, Right, Middle)");
                                    continue;
                                }

                                inputComponent.Mouse[action] = button;
                            }
                        }
                        break;
                }
            }
            }
            catch (Exception ex)
            {
                _ = new GameWontRun(YTBErrors.InputParseFailed, sceneName, name, nameof(InputComponent), "", $"Error al parsear InputComponent: {ex.Message}", "Revise que todas las propiedades del InputComponent (InputsInUse, GamePadIndex, KeyboardMappings, MouseMappings) sean válidas");
            }

            return inputComponent;
        }

        /// <summary>
        /// Convierte un componente YTB en un FontComponent2D.
        /// </summary>
        /// <param name="component">El componente YTB que contiene las propiedades del font.</param>
        /// <param name="entityName">El nombre de la entidad a la que pertenece el componente.</param>
        /// <returns>Retorna un FontComponent2D configurado según las propiedades del componente YTB.</returns>
        /// <summary>
        /// Converts YTB component data into a font component.
        /// Convierte datos de componente YTB en un componente de fuente.
        /// </summary>
        /// <param name="component">Component data. Datos del componente.</param>
        /// <param name="entityName">Entity name. Nombre de la entidad.</param>
        /// <returns>Font component. Componente de fuente.</returns>
        private static FontComponent2D ConvertToFont(YTBComponents component, string entityName, string sceneName = "")
        {
            FontComponent2D fontComponent = new FontComponent2D();

            try
            {
                string texto = component.Propiedades.FirstOrDefault(x => x.Item1 == nameof(fontComponent.Texto))?.Item2 ?? string.Empty;
                string font = component.Propiedades.FirstOrDefault(x => x.Item1 == nameof(fontComponent.Font))?.Item2 ?? string.Empty;

                if (string.IsNullOrWhiteSpace(font))
                {
                    _ = new GameWontRun(YTBErrors.FontParseFailed, sceneName, entityName, nameof(FontComponent2D), "Font", "La propiedad Font está vacía o es nula", "Asigne una fuente válida en la propiedad Font del componente FontComponent2D");
                }

                fontComponent.Texto = texto;
                fontComponent.Font = font;

                return fontComponent;
            }
            catch (Exception ex)
            {
                _ = new GameWontRun(YTBErrors.FontParseFailed, sceneName, entityName, nameof(FontComponent2D), "", $"Error al parsear FontComponent2D: {ex.Message}", "Revise que todas las propiedades del FontComponent2D sean válidas");
                return fontComponent;
            }
        }

        /// <summary>
        /// Convierte el formato YTB del componente de shader a un ShaderComponent2D.
        /// </summary>
        /// <param name="component">Componente YTB con las propiedades del shader</param>
        /// <param name="entityName">Nombre de la entidad para mensajes de error</param>
        /// <returns>ShaderComponent2D configurado con el shader y sus parámetros</returns>
        /// <summary>
        /// Converts YTB component data into a shader component.
        /// Convierte datos de componente YTB en un componente de shader.
        /// </summary>
        /// <param name="component">Component data. Datos del componente.</param>
        /// <param name="entityName">Entity name. Nombre de la entidad.</param>
        /// <returns>Shader component. Componente de shader.</returns>
        private static ShaderComponent ConvertToShader(YTBComponents component, string entityName, string sceneName = "")
        {
            try
            {
                // Obtener ShaderPath (obligatorio)
                string shaderPath = component.Propiedades.FirstOrDefault(x => x.Item1 == "ShaderPath")?.Item2 ?? string.Empty;
                
                if (string.IsNullOrWhiteSpace(shaderPath))
                {
                    _ = new GameWontRun(YTBErrors.ShaderWontBeParse, sceneName, entityName, nameof(ShaderComponent), "ShaderPath", "ShaderPath está vacío o es nulo", "Asigne una ruta válida al shader en la propiedad ShaderPath del componente ShaderComponent");
                    return default;
                }

                // Cargar el shader usando ShaderManager
                Effect effect = ShaderManager.LoadShader(shaderPath);

                bool isActive = true;
                string isActiveStr = component.Propiedades.FirstOrDefault(x => x.Item1 == "IsActive")?.Item2;
                if (!string.IsNullOrEmpty(isActiveStr))
                {
                    bool.TryParse(isActiveStr, out isActive);
                }

                // Crear el componente
                ShaderComponent shaderComponent = new ShaderComponent(effect, isActive);

                string shaderParams = component.Propiedades.FirstOrDefault(x => x.Item1 == "params")?.Item2 ?? string.Empty;

                if (String.IsNullOrEmpty(shaderParams)) return shaderComponent;

                foreach(string param in shaderParams.Split(";"))
                {
                    if (String.IsNullOrEmpty(param)) continue;

                    string key = param.Split("=")[0];
                    int value = int.Parse(param.Split("=")[1]);

                    shaderComponent.Effect.Parameters[key].SetValue(value);
                }

                return shaderComponent;
            }
            catch (Exception ex)
            {
                _ = new GameWontRun(YTBErrors.ShaderWontBeParse, sceneName, entityName, nameof(ShaderComponent), "", $"Error al parsear ShaderComponent: {ex.Message}", "Revise que la ruta del shader y sus parámetros sean válidos");
                return default;
            }
        }
        #endregion

        #region ModelComponent3D
        /// <summary>
        /// Converts YTB component data into a 3D model component.
        /// Convierte datos de componente YTB en un componente de modelo 3D.
        /// </summary>
        /// <param name="component">Component data. Datos del componente.</param>
        /// <param name="entityName">Entity name. Nombre de la entidad.</param>
        /// <returns>Model3D component. Componente de modelo 3D.</returns>
        private static ModelComponent3D ConvertToModel3D(YTBComponents component, string entityName, string sceneName = "")
        {
            try
            {
                string modelPath = component.Propiedades.FirstOrDefault(x => x.Item1 == "ModelPath")?.Item2 ?? string.Empty;
                
                if (string.IsNullOrEmpty(modelPath))
                {
                    EngineUISystem.SendLog($"Warning: ModelComponent3D for entity {entityName} has empty ModelPath. Returning default component.");
                    return default;
                }

                Model model = ContentManager.Load<Model>(modelPath);

                bool isVisible = true;
                string isVisibleStr = component.Propiedades.FirstOrDefault(x => x.Item1 == "IsVisible")?.Item2;
                if (!string.IsNullOrEmpty(isVisibleStr))
                {
                    bool.TryParse(isVisibleStr, out isVisible);
                }

                ModelComponent3D modelComponent = new ModelComponent3D(model)
                {
                    IsVisible = isVisible,

                };

                string radius = component.Propiedades.FirstOrDefault(x => x.Item1 == "SphereRadius")?.Item2;
                    if (float.TryParse(radius, out float r))
                    {
                        modelComponent.RadiusSphere = r;
                    }

                string offsetSphere = component.Propiedades.FirstOrDefault(x => x.Item1 == "OffsetSphere").Item2;

                Vector3 offset = Vector3.Zero;
                string[] dims = offsetSphere.Split(",");
                if (!String.IsNullOrEmpty(offsetSphere))
                {
                    for (int i = 0; i < dims.Length; i++)
                    {
                        {
                            switch (i)
                            {
                                case 0:
                                    offset.X = float.Parse(dims[0]);
                                    break;
                                case 1:
                                    offset.Y = float.Parse(dims[1]);
                                    break;
                                case 2:
                                    offset.Z = float.Parse(dims[2]);
                                    break;
                                default:
                                    break;
                            }
                        }

                    }
                }

                modelComponent.SphereOffset = offset;

                EngineUISystem.SendLog($"ModelComponent3D loaded successfully for entity {entityName}: {modelPath}");
                return modelComponent;
            }
            catch (Exception ex)
            {
                _ = new GameWontRun(YTBErrors.Model3DLoadFailed, sceneName, entityName, nameof(ModelComponent3D), "ModelPath", $"Error al cargar ModelComponent3D: {ex.Message}", "Revise que la ruta del modelo 3D sea válida y el archivo exista en el Content Pipeline");
                return default;
            }
        }
        #endregion
    }
}
