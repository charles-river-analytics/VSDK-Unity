Shader "Custom/LineRendererOverlay" 
{
    Properties 
    {
         _Color ("Main Color", Color) = (1,1,1,1) 
    }
    Category 
	{
		SubShader 
		{ 
			Tags 
			{ 
				"Queue" = "Overlay" 
			}
			
			BindChannels 
			{
				Bind "color", color 
			}

			Pass
			{
				ZTest Greater
				Lighting Off
				Color [color]
			}
			Pass
			{
				ZTest Less
			}
		}
	}
}