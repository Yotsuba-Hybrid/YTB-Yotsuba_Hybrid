using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Hexa.NET.ImGui;
using Hexa.NET.ImNodes;

// Alias para evitar colisiones con Microsoft.Xna.Framework.Vector2/3/4
using Num = System.Numerics;

using YotsubaEngine;
using YotsubaEngine.Core.Component.C_3D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Graphics;
using MonoGameGum;
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;

namespace SandBoxGame.Core.Systems
{
    // --- CLASES DEL SISTEMA SIMPLE ---
    public class SimpleNode
    {
        public int Id;
        public string Title;
        public int InputPinId;
        public int OutputPinId;
        public Num.Vector2 ManualPositionValue;
        public Num.Vector2 FinalEffectiveValue;
        public int PositionInputPinId;
        public int PositionOutputPinId;
    }

    public class SimpleLink
    {
        public int Id;
        public int StartPinId;
        public int EndPinId;
    }

    public class CustomExampleSystem : IRenderSystem
    {
        GumService GumUI => GumService.Default;

        private List<SimpleNode> _nodes = new List<SimpleNode>();
        private List<SimpleLink> _links = new List<SimpleLink>();
        private int _currentId = 1;

        // Instancia del editor avanzado (Pixi Style)
        private YTBHybrid_IDE.VisualProgramming.AdvancedNodeEditor _advancedEditor;

        public CustomExampleSystem()
        {
            _nodes.Add(new SimpleNode
            {
                Id = _currentId++,
                Title = "Transform: Object A",
                InputPinId = _currentId++,
                OutputPinId = _currentId++,
                ManualPositionValue = new Num.Vector2(0, 0),
                PositionInputPinId = _currentId++,
                PositionOutputPinId = _currentId++
            });

            _advancedEditor = new YTBHybrid_IDE.VisualProgramming.AdvancedNodeEditor();
        }

        public override void InitializeSystem(EntityManager entities)
        {
            EntityManager = entities;
            //var gumProject = GumUI.Initialize(YTBGlobalState.Game, "Gum/GumProject.gumx");
            //var screen = new YTBTESTRuntime();
            //screen.AddToRoot();
        }

        public override void SharedEntityInitialize(ref Yotsuba Entidad) { }
        public override void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time) { }

        public override void UpdateSystem(GameTime gameTime)
        {
            //GumUI.Update(gameTime);
        }

        #region Renderizado
        public override void Render2D(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //YTBGame.GuiRenderer.BeginLayout(gameTime);

            //ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Num.Vector2(0, 0));

            //// Renderizamos el editor avanzado con el estilo PixiEditor
            //_advancedEditor.Draw();

            //ImGui.PopStyleVar();

            //YTBGame.GuiRenderer.EndLayout();
            //GumUI.Draw();
        }

        public override void Render3D(GameTime gameTime) { }
        public override void Dispose() { }
        #endregion
    }
}


namespace YTBHybrid_IDE.VisualProgramming
{
    // Tipos de datos basados en la imagen de PixiEditor
    public enum DataType { ImageLayer, Position, Size, Boolean, Filter, Default }

    public static class PixiPalette
    {
        // Colores base de la interfaz
        public static readonly uint Background = ImGui.ColorConvertFloat4ToU32(new Num.Vector4(0.12f, 0.12f, 0.13f, 1.0f)); // Gris oscuro
        public static readonly uint TitleBar = ImGui.ColorConvertFloat4ToU32(new Num.Vector4(0.16f, 0.16f, 0.17f, 1.0f));   // Gris un poco más claro
        public static readonly uint Outline = ImGui.ColorConvertFloat4ToU32(new Num.Vector4(0.1f, 0.1f, 0.1f, 1.0f));       // Borde casi negro
        public static readonly uint LinkOrange = ImGui.ColorConvertFloat4ToU32(new Num.Vector4(0.9f, 0.5f, 0.0f, 1.0f));    // Naranja Pixi

        // Colores de los Pines
        public static readonly Num.Vector4 PinOrange = new Num.Vector4(0.9f, 0.5f, 0.0f, 1.0f);   // Output, Background
        public static readonly Num.Vector4 PinPurple = new Num.Vector4(0.6f, 0.3f, 0.7f, 1.0f);   // Canvas Position
        public static readonly Num.Vector4 PinPink = new Num.Vector4(0.8f, 0.3f, 0.6f, 1.0f);     // Size
        public static readonly Num.Vector4 PinBlue = new Num.Vector4(0.3f, 0.6f, 0.9f, 1.0f);     // Checkboxes / Visibilidad
        public static readonly Num.Vector4 PinRed = new Num.Vector4(0.8f, 0.2f, 0.2f, 1.0f);      // Filters
    }

    public class Pin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DataType Type { get; set; }
        public bool IsOutput { get; set; }
    }

    public enum NodeType { Layer, Output }

    public class Node
    {
        public int Id { get; set; }
        public NodeType Type { get; set; }
        public string Title { get; set; }
        public List<Pin> Pins { get; set; } = new List<Pin>();

        // Propiedades de la capa (Valores de ImGui)
        public float Opacity = 1.0f;
        public bool IsVisible = true;
        public int BlendModeIndex = 0;
        public bool MaskIsVisible = true;
    }

    public class Link { public int Id, StartPinId, EndPinId; }

    public class AdvancedNodeEditor
    {
        private List<Node> _nodes = new List<Node>();
        private List<Link> _links = new List<Link>();
        private int _currentId = 1;

        private string[] _blendModes = { "Normal", "Multiply", "Screen", "Overlay" };

        public AdvancedNodeEditor() { SpawnPixiEditorScene(); }

        public void Draw()
        {
            ImGui.Begin("PixiEditor Style Graph");

            // --- ESTILIZADO GLOBAL TIPO PIXIEDITOR ---
            ImNodes.PushStyleVar(ImNodesStyleVar.NodeCornerRounding, 4.0f); // Bordes menos redondeados
            ImNodes.PushStyleVar(ImNodesStyleVar.NodePadding, new Num.Vector2(8f, 8f));
            ImNodes.PushStyleVar(ImNodesStyleVar.LinkThickness, 3.0f);
            ImNodes.PushStyleVar(ImNodesStyleVar.PinCircleRadius, 4.0f); // Pines más pequeños

            // Colores del Canvas
            ImNodes.PushColorStyle(ImNodesCol.GridBackground, ImGui.ColorConvertFloat4ToU32(new Num.Vector4(0.15f, 0.15f, 0.15f, 1.0f)));
            ImNodes.PushColorStyle(ImNodesCol.GridLine, ImGui.ColorConvertFloat4ToU32(new Num.Vector4(0.18f, 0.18f, 0.18f, 1.0f)));
            
            // Colores por defecto del nodo
            ImNodes.PushColorStyle(ImNodesCol.NodeBackground, PixiPalette.Background);
            ImNodes.PushColorStyle(ImNodesCol.NodeBackgroundHovered, PixiPalette.Background + 0x050505);
            ImNodes.PushColorStyle(ImNodesCol.NodeBackgroundSelected, PixiPalette.Background + 0x101010);
            ImNodes.PushColorStyle(ImNodesCol.NodeOutline, PixiPalette.Outline);
            ImNodes.PushColorStyle(ImNodesCol.TitleBar, PixiPalette.TitleBar);
            ImNodes.PushColorStyle(ImNodesCol.TitleBarHovered, PixiPalette.TitleBar);
            ImNodes.PushColorStyle(ImNodesCol.TitleBarSelected, PixiPalette.TitleBar);

            // Color del Cable
            ImNodes.PushColorStyle(ImNodesCol.Link, PixiPalette.LinkOrange);
            ImNodes.PushColorStyle(ImNodesCol.LinkHovered, PixiPalette.LinkOrange + 0x222222);
            ImNodes.PushColorStyle(ImNodesCol.LinkSelected, PixiPalette.LinkOrange + 0x444444);

            ImNodes.BeginNodeEditor();

            foreach (var node in _nodes)
            {
                ImNodes.BeginNode(node.Id);

                // HEADER
                ImNodes.BeginNodeTitleBar();
                ImGui.TextUnformatted(node.Title);
                ImNodes.EndNodeTitleBar();

                // CUERPO DEL NODO (Layout denso a 1 columna)
                ImGui.PushItemWidth(100f);

                // 1. DIBUJAR PINES DE SALIDA (Arriba a la derecha)
                foreach (var pin in node.Pins.Where(p => p.IsOutput))
                {
                    DrawPixiPin(pin);
                }

                if (node.Type == NodeType.Layer)
                {
                    ImGui.Spacing();
                    
                    // 2. PROPIEDADES MEZCLADAS CON PINES DE ENTRADA
                    DrawPixiInputPin(node, "Background", DataType.ImageLayer);
                    
                    ImGui.Text("Opacity"); ImGui.SameLine(70);
                    ImGui.InputFloat("##op" + node.Id, ref node.Opacity, 0, 0, "%.1f");

                    DrawPixiInputPin(node, "Is visible", DataType.Boolean);
                    ImGui.SameLine(); ImGui.Checkbox("##vis" + node.Id, ref node.IsVisible);

                    ImGui.Text("Blend mode"); ImGui.SameLine();
                    ImGui.Combo("##bm" + node.Id, ref node.BlendModeIndex, _blendModes, _blendModes.Length);

                    DrawPixiInputPin(node, "Mask", DataType.ImageLayer);

                    DrawPixiInputPin(node, "Mask is visible", DataType.Boolean);
                    ImGui.SameLine(); ImGui.Checkbox("##mvis" + node.Id, ref node.MaskIsVisible);

                    DrawPixiInputPin(node, "Filters", DataType.Filter);
                }
                else if (node.Type == NodeType.Output)
                {
                    DrawPixiInputPin(node, "Background", DataType.ImageLayer);
                }

                ImGui.PopItemWidth();

                // 3. THUMBNAIL (Simulación del fondo de cuadros)
                ImGui.Spacing();
                Num.Vector2 cursorPos = ImGui.GetCursorScreenPos();
                Num.Vector2 size = new Num.Vector2(140, 80);
                
                // Dibujamos un rectángulo gris oscuro simulando el área de la imagen
                ImGui.GetWindowDrawList().AddRectFilled(cursorPos, cursorPos + size, ImGui.ColorConvertFloat4ToU32(new Num.Vector4(0.2f, 0.2f, 0.2f, 1.0f)), 4f);
                ImGui.GetWindowDrawList().AddRect(cursorPos, cursorPos + size, PixiPalette.Outline, 4f);
                
                // Espacio en blanco para que ImGui sepa que ocupamos esta área
                ImGui.Dummy(size);

                ImNodes.EndNode();
            }

            foreach (var link in _links) ImNodes.Link(link.Id, link.StartPinId, link.EndPinId);

            ImNodes.EndNodeEditor();

            // Restaurar todos los estilos
            ImNodes.PopColorStyle(); ImNodes.PopColorStyle(); ImNodes.PopColorStyle(); ImNodes.PopColorStyle();
            ImNodes.PopColorStyle(); ImNodes.PopColorStyle(); ImNodes.PopColorStyle(); ImNodes.PopColorStyle();
            ImNodes.PopColorStyle(); ImNodes.PopColorStyle(); ImNodes.PopColorStyle(); ImNodes.PopColorStyle();
            ImNodes.PopStyleVar(); ImNodes.PopStyleVar(); ImNodes.PopStyleVar(); ImNodes.PopStyleVar();

            // Creación de cables
            int startAttr = 0; int endAttr = 0;
            if (ImNodes.IsLinkCreated(ref startAttr, ref endAttr))
            {
                _links.Add(new Link { Id = _currentId++, StartPinId = startAttr, EndPinId = endAttr });
            }

            ImGui.End();
        }

        // --- MÉTODOS DE AYUDA VISUAL ---

        private void DrawPixiPin(Pin pin)
        {
            SetPinColor(pin.Type);
            ImNodes.BeginOutputAttribute(pin.Id, ImNodesPinShape.CircleFilled);
            
            // En PixiEditor, los outputs están alineados a la derecha con el texto a su izquierda
            float textWidth = ImGui.CalcTextSize(pin.Name).X;
            float columnWidth = 140f; // Ancho fijo simulado para el nodo
            
            ImGui.Indent(columnWidth - textWidth - 10f);
            ImGui.TextUnformatted(pin.Name);
            ImGui.Unindent();
            
            ImNodes.EndOutputAttribute();
            ImNodes.PopColorStyle(); ImNodes.PopColorStyle();
        }

        // Dibuja un pin de entrada (como "Background" o "Mask") que va a la izquierda
        private void DrawPixiInputPin(Node node, string name, DataType type)
        {
            // Creamos o buscamos el pin en la lista del nodo
            Pin pin = node.Pins.FirstOrDefault(p => p.Name == name && !p.IsOutput);
            if (pin == null)
            {
                pin = new Pin { Id = _currentId++, Name = name, Type = type, IsOutput = false };
                node.Pins.Add(pin);
            }

            SetPinColor(type);
            ImNodes.BeginInputAttribute(pin.Id, ImNodesPinShape.CircleFilled);
            ImGui.TextUnformatted(pin.Name);
            ImNodes.EndInputAttribute();
            ImNodes.PopColorStyle(); ImNodes.PopColorStyle();
        }

        private void SetPinColor(DataType type)
        {
            Num.Vector4 color;
            switch (type)
            {
                case DataType.ImageLayer: color = PixiPalette.PinOrange; break;
                case DataType.Position: color = PixiPalette.PinPurple; break;
                case DataType.Size: color = PixiPalette.PinPink; break;
                case DataType.Boolean: color = PixiPalette.PinBlue; break;
                case DataType.Filter: color = PixiPalette.PinRed; break;
                default: color = new Num.Vector4(1, 1, 1, 1); break;
            }

            uint uColor = ImGui.ColorConvertFloat4ToU32(color);
            ImNodes.PushColorStyle(ImNodesCol.Pin, uColor);
            ImNodes.PushColorStyle(ImNodesCol.PinHovered, uColor + 0x202020);
        }

        // --- FÁBRICA DE ESCENA (Igual a la imagen) ---
        private void SpawnPixiEditorScene()
        {
            var layer1 = CreateLayerNode("Capa base");
            var layer2 = CreateLayerNode("Barca");
            var outputNode = new Node { Id = _currentId++, Type = NodeType.Output, Title = "Output" };
            
            _nodes.Add(layer1);
            _nodes.Add(layer2);
            _nodes.Add(outputNode);

            // Nota: En una implementación real conectaríamos los IDs correctamente.
            // Por ahora, al arrancar la UI el usuario podrá arrastrar los cables y se verán idénticos.
        }

        private Node CreateLayerNode(string title)
        {
            var node = new Node { Id = _currentId++, Type = NodeType.Layer, Title = title };
            
            // Outputs superiores
            node.Pins.Add(new Pin { Id = _currentId++, Name = "Output", Type = DataType.ImageLayer, IsOutput = true });
            node.Pins.Add(new Pin { Id = _currentId++, Name = "Without filters", Type = DataType.ImageLayer, IsOutput = true });
            node.Pins.Add(new Pin { Id = _currentId++, Name = "Raw", Type = DataType.ImageLayer, IsOutput = true });
            node.Pins.Add(new Pin { Id = _currentId++, Name = "Canvas Position", Type = DataType.Position, IsOutput = true });
            node.Pins.Add(new Pin { Id = _currentId++, Name = "Center Position", Type = DataType.Position, IsOutput = true });
            node.Pins.Add(new Pin { Id = _currentId++, Name = "Size", Type = DataType.Size, IsOutput = true });

            return node;
        }
    }
}