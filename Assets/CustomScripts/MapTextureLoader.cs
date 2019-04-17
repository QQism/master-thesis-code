using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mapbox.Map;
using Mapbox.Unity;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;

public class MapTextureLoader : MonoBehaviour {

	public AbstractMap _map;

	// Use this for initialization
	void Start () {
		
	}

	void Awake() 
	{
		//_map.OnInitialized += loadMapCone;
	}

    void loadMapCone()
    {
        var parameters = new Tile.Parameters();
        parameters.Fs = MapboxAccess.Instance;
        // lat long -37.8142, 144.9632
		Vector2d latlong = _map.WorldToGeoPosition(transform.position);
        //UnwrappedTileId tileId = Conversions.LatitudeLongitudeToTileId(-37.8142, 144.9632, 10);
		// Current latlong: 144.96470,-37.82177
		Debug.Log("Current latlong: " + latlong);
        UnwrappedTileId tileId = Conversions.LatitudeLongitudeToTileId(latlong.x, latlong.y, 10);

        parameters.Id = new CanonicalTileId(tileId.Z, tileId.X, tileId.Y);
        parameters.MapId = "mapbox://styles/quangquach/cjonlkxkg3it52snz3q5yfnqd";
        var rasterTile = new RasterTile();

        Debug.Log("Start to fetch Mapbox");
        rasterTile.Initialize(parameters, (Action)(() =>
        {
            if (!rasterTile.HasError)
            {
                Debug.Log("Mapbox image loaded, saving");
                var texture = new Texture2D(0, 0);
                texture.LoadImage(rasterTile.Data);

                string filePath = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[] {
                    Application.dataPath,
                    "maptest.png"
                });

                File.WriteAllBytes(filePath, rasterTile.Data);
            }
            else
            {
                Debug.Log("Mapbox failed to load image!");
            }
        }));
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
