using NUnit.Framework;

namespace HouraiTeahouse.SmashBrew {

    internal class SceneDataTest : AbstractDataTest<SceneData> {

        [Test]
        public void PreviewImageTest() { Check(s => s.PreviewImage.Load()); }

        [Test]
        public void IconTest() { Check(s => s.Icon.Load()); }

    }

}