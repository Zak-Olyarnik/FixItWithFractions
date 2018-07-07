// Taken from: http://wiki.unity3d.com/index.php/DepthMask
Shader "Custom/DepthMask" {
	SubShader {
		// Render the mask after regular geometry, but before masked geometry and
         // transparent things.
 
        Tags {"Queue" = "Transparent" }
 
		//http://www.codingwithunity.com/2016/01/stencil-bufferfor-2d.html
        Cull Off    
        ZWrite Off
      
        Blend Zero One

        Pass
        {

			Stencil
			{
				Ref 4
				Comp Always
				Pass replace
			}
		}
	}
}
