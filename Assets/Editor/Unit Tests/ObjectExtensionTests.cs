using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class ObjectExtensionTests {

    [Test]
    public void BasicCopyTest() {
        GameObject test1 = new GameObject("test 1", typeof (SpriteRenderer));
        GameObject test2 = test1.Copy();
        Assert.True(test1 != test2);
        Assert.True(test1.transform.position == test2.transform.position);
        Assert.True(test1.transform.rotation == test2.transform.rotation);
        Assert.True(test2.GetComponent<SpriteRenderer>());
        Assert.True(test2.name.Contains("test 1"));
    }

    [Test]
    public void PositionCopyTest() {
        GameObject test1 = new GameObject("test 1", typeof (SpriteRenderer));
        GameObject test2 = test1.Copy(Vector3.one);
        Assert.True(test1 != test2);
        Assert.True(Vector3.one == test2.transform.position);
        Assert.True(test1.transform.rotation == test2.transform.rotation);
        Assert.True(test2.GetComponent<SpriteRenderer>());
        Assert.True(test2.name.Contains("test 1"));
    }

    [Test]
    public void PosRotCopyTest() {
        GameObject test1 = new GameObject("test 1", typeof (SpriteRenderer));
        GameObject test2 = test1.Copy(Vector3.one, Quaternion.Euler(Vector3.one));
        Assert.True(test1 != test2);
        Assert.True(Vector3.one == test2.transform.position);
        Assert.True(Quaternion.Euler(Vector3.one) == test2.transform.rotation);
        Assert.True(test2.GetComponent<SpriteRenderer>());
        Assert.True(test2.name.Contains("test 1"));
    }

    [Test]
    public void NullCopyTest() {
        GameObject nullObject = null;
        Assert.DoesNotThrow(() => {
                                nullObject.Copy();
                            });
        GameObject copy = nullObject.Copy();
        Assert.True(copy == null);
    }
    
    [Test]
    public void BasicDestroyTest() {
        GameObject test = new GameObject();
        test.Destroy();
        Assert.True(test == null);
        Object.DestroyImmediate(test);
    }

    [Test]
    public void CollectionDestroyTest() {
        GameObject[] gameObjectTest = new GameObject[200];
        for(var i = 0; i < gameObjectTest.Length; i++)
            gameObjectTest[i] = new GameObject();
        gameObjectTest.DestroyAll();
        for (var i = 0; i < gameObjectTest.Length; i++) {
            Assert.True(gameObjectTest[i] == null);
            Object.DestroyImmediate(gameObjectTest[i]);
        }
    }

    [Test]
    public void NullDestroyTest() {
        GameObject nullObject = null;
        Assert.DoesNotThrow(() => 
        {
            nullObject.Destroy();
        });
    }

    [Test]
    public void NullCollectionDestroyTest() {
        HashSet<GameObject> nullCollection = null;
        Assert.DoesNotThrow(() =>
        {
            nullCollection.DestroyAll();
        });
    }
}
