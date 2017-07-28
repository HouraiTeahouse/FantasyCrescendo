using UnityEngine;

namespace HouraiTeahouse.AssetBundles {

    // Loaded assetBundle contains the references count which can be used to unload dependent assetBundles automatically.
    public class LoadedAssetBundle {

        public BundleMetadata Metadata { get; private set; }
        public AssetBundle AssetBundle { get; private set; }
        public int ReferencedCount { get; internal set; }

        public LoadedAssetBundle(BundleMetadata metadata, AssetBundle assetBundle) {
            Metadata = Argument.NotNull(metadata);
            AssetBundle = Argument.NotNull(assetBundle);
            ReferencedCount = 1;
        }

    }

}
