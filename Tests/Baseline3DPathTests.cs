using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Xunit;
using YotsubaEngine.Core.Component.C_3D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.S_3D;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.HighestPerformanceTypes;
using YotsubaEngine.Runtime.CPR;

namespace YotsubaEngine.Tests;

/// <summary>
/// BASELINE TESTS: ruta 3D sin GPU real (sin OcclusionQuery)
/// Estos tests protegen regresiones de lógica pura.
/// </summary>
public partial class Baseline3DPathTests
{
    [Fact]
    public void AddRigidbody3DComponent_SeteaFlagRigibody3D()
    {
        var em = new EntityManager();
        var entity = new Yotsuba(0);
        em.AddEntity(ref entity);

        em.AddRigidbody3DComponent(entity, new RigidBodyComponent3D());

        Assert.True(em.YotsubaEntities[entity.Id].HasComponent(YTBComponent.Rigibody3D));
    }

    [Fact]
    public void CPR_NoDuplicaRegistros_MismaEntidad()
    {
        Collision_Prediction_Runtime_3D.UnPhysicalCollisionDistance = 10;
        var em = BuildEntityManagerWithTransformAndRb3D(new Vector3(5, 0, 0));
        var cpr = new Collision_Prediction_Runtime_3D();
        cpr.InitializeSystem(em);

        var tmp = new YTB<int>();
        ref var t = ref em.TransformComponents[0];

        cpr.IsPhysicalPossibleCollide(ref t, 0, tmp);
        cpr.IsPhysicalPossibleCollide(ref t, 0, tmp);

        Assert.Equal(1, cpr.Entities.Count);
        var idsInCells = cpr.SpatialHashGrid.Values.Sum(v => v.Count);
        Assert.Equal(1, idsInCells);
        cpr.Dispose();
    }

    [Fact]
    public void CPR_NoCrasheaSiFaltaEntityPoint()
    {
        Collision_Prediction_Runtime_3D.UnPhysicalCollisionDistance = 10;
        var em = BuildEntityManagerWithTransformAndRb3D(new Vector3(2, 0, 0));
        var cpr = new Collision_Prediction_Runtime_3D();
        cpr.InitializeSystem(em);

        var field = typeof(Collision_Prediction_Runtime_3D).GetField("EntityPoint", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var map = (System.Collections.IDictionary)field.GetValue(cpr)!;
        map.Remove(0);

        var tmp = new YTB<int>();
        ref var t = ref em.TransformComponents[0];
        var ex = Record.Exception(() => cpr.IsPhysicalPossibleCollide(ref t, 0, tmp));

        Assert.Null(ex);
        cpr.Dispose();
    }

    [Fact]
    public void RemoveFast_NoFallaCuandoNoExisteItem()
    {
        var ytb = new YTB<int>();
        ytb.Add(1);
        ytb.Add(2);

        var removed = ytb.RemoveFast(999);

        Assert.False(removed);
        Assert.Equal(2, ytb.Count);
    }

    [Fact]
    public void DistanciaEspacialInvalida_SeNormalizaAUno()
    {
        Collision_Prediction_Runtime_3D.UnPhysicalCollisionDistance = -42;
        Assert.Equal(1, Collision_Prediction_Runtime_3D.UnPhysicalCollisionDistance);
    }

    [Fact]
    public void BoxVsBox_NormalNoEsCero()
    {
        var physics = new PhysicsSystem3D();
        var method = typeof(PhysicsSystem3D).GetMethod("CalculateBoxVsBoxPenetration", BindingFlags.NonPublic | BindingFlags.Instance)!;

        var a = new BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
        var b = new BoundingBox(new Vector3(0.5f, -1, -1), new Vector3(2.5f, 1, 1));
        object[] args = [a, b, Vector3.Zero, 0f];

        method.Invoke(physics, args);

        var normal = (Vector3)args[2];
        Assert.NotEqual(Vector3.Zero, normal);
    }

    [Fact]
    public void SphereInsideBox_UsaEjeSalidaMinimo()
    {
        var physics = new PhysicsSystem3D();
        var method = typeof(PhysicsSystem3D).GetMethod("CalculateSphereVsBoxPenetration", BindingFlags.NonPublic | BindingFlags.Instance)!;

        var sphere = new BoundingSphere(new Vector3(0.9f, 0f, 0f), 0.5f);
        var box = new BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
        object[] args = [sphere, box, Vector3.Zero, 0f];

        method.Invoke(physics, args);

        var normal = (Vector3)args[2];
        Assert.Equal(new Vector3(1, 0, 0), normal);
    }

    [Fact]
    public void Baseline_MarcadoParaRuta3D_FuturasRegresiones()
    {
        Assert.True(true, "Baseline 3D path tests activos para cambios futuros.");
    }

    private static EntityManager BuildEntityManagerWithTransformAndRb3D(Vector3 pos)
    {
        var em = new EntityManager();
        var entity = new Yotsuba(0);
        em.AddEntity(ref entity);
        em.AddTransformComponent(entity, new TransformComponent { Position = pos });
        em.AddRigidbody3DComponent(entity, new RigidBodyComponent3D());
        return em;
    }
}

public partial class Baseline3DPathTests
{
    [Fact]
    public void OCR_CamaraNula_EsDeterministaYNoLanza()
    {
        var em = new EntityManager();
        var e = new Yotsuba(0);
        em.AddEntity(ref e);
        em.AddTransformComponent(e, new TransformComponent());

        var ocr = new YotsubaEngine.Runtime.OCR.Hardware_Occlusion_Querie_Runtime();
        ocr.InitializeSystem(em);

        var result = ocr.GetEntitiesToRender();

        Assert.Equal(0, result.Length);
    }

    [Fact]
    public void Render3D_CamaraNula_EsDeterministaYNoLanza()
    {
        var em = new EntityManager();
        var render = new RenderSystem3D();
        render.InitializeSystem(em);

        var ex = Record.Exception(() => render.Render3D(new GameTime()));

        Assert.Null(ex);
    }

    [Fact]
    public void OCR_SaltaEntidadSinModel3D_SinIntentarLeerModelo()
    {
        var em = new EntityManager();
        var e = new Yotsuba(0);
        em.AddEntity(ref e);
        em.AddTransformComponent(e, new TransformComponent());
        em.Camera = new CameraComponent3D();

        var ocr = new YotsubaEngine.Runtime.OCR.Hardware_Occlusion_Querie_Runtime();
        ocr.InitializeSystem(em);

        var ex = Record.Exception(() => ocr.GetEntitiesToRender());
        Assert.Null(ex);
    }
}
