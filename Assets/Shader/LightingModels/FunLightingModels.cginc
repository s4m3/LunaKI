	half4 LightingCustomLambert (SurfaceOutput s, half3 lightDir, half atten) {
			//Tricky: Durch die simple Arbeitsweise der Grafikhardware haben die Normalen
			//zwischen den Eckpunkten nicht mehr die Länge 1. Die Hardware erzeugt zwischen
			//den Eckpunkten des Meshes auf die einfachste Art und Weise Zwischenwerte durch
			//"lineare Interpolation" (lerp). Dadurch verlieren die Normalen aber an Länge.
			//Wir müssen hier also eine "Reparatur" vornehmen - nur dann erscheinen
			//eckige Körper als abgerundet (s. Folien). Lässt man diese Zeile weg, erhält
			//man einen "Gouraud" oder "Vertex Lit" Shading Effekt.
			//
			//Für den unbedarften Benutzer ist ein unnormierter (Länge !=1) Vektor mit dem Namen
			//"Normal" schon sehr gewöhnungsbedürftig ;-)
			//
			//Nach dieser Zeile können wir uns aber sicher sein, dass der Vektor tatsächlich
			//die Länge 1 hat.  
			half3 normalDir = normalize(s.Normal);
			
			//Diffuse Beleuchtung
			//Je flacher eine Fläche dem Licht gegenüber steht, desto mehr verteilt sich
			//der Lichtstrom über die Fläche - die einfallende Lichtmenge wird geringer.
			//Das "Punktprodukt" wird für Normalen, die vom Lichtvektor abgewandt sind
			//negativ. Dies würde zu eine negativen Lichteinfallswert führen - wir
			//begrenzen daher den Lichteinfall durch den "max" Term auf minimal 0.
			//So garantieren wir, dass die Rückseite des Objekts die Farbe schwarz erhält.
			//Abschliessend multiplizieren wir mit der entfernungsbasierten Lichtabschwächung
			//"attenuation", die uns praktischerweise von der Unity-Engine vorberechnet wurde.
			//Als Ergebnis erhalten wir die einfallende Lichtmenge.  
			half incomingLightAmount = max(0, dot(normalDir, lightDir)) * atten;
			
			//Die Lichtquelle emittiert im RGB Farbrum - wir skalieren die emittierten RGB Werte
			//mit dem vorher berechneten Lichtmenge, die zwischen Lichtquelle und Oberfläche
			//transportiert wird und erhalten die individuellen Lichteinfallswerte für
			//RGB.
			half4 incomingColor = incomingLightAmount * _LightColor0;
			
			//Das einfallende Licht, interagiert mit dem Material der Oberfläche. Die
			//Oberfläche reflektiert und absorbiert jeweils einen Teil der Oberfläche.
			//Das Verhältnis von Reflektion zur Absorbtion wird durch die Albedo beschrieben.
			//Ein Albedowert im RGB Raum von (0.7, 0.3, 0.2) reflektiert 70% des einfallenden
			//roten Lichts, 30% des grünen und 20% des blauen Lichtes.
			//Um die reflektierte Lichtmenge zu erhalten wird das einfallende Licht (RGB Vektor)
			//mit den prozentualen Anteilen der Albedo elementweise multipliziert.
			half4 outgoingColor = incomingColor * half4(s.Albedo, s.Alpha); 
			
			return outgoingColor;
	}
	
	half _halfLambertPower;
	
	half4 LightingCustomHalfLambert (SurfaceOutput s, half3 lightDir, half atten) {
			half3 normalDir = normalize(s.Normal);
			
			
			half incomingLightAmount = pow(0.5 * (dot(normalDir, lightDir) + 1), _halfLambertPower)  * atten;
			half4 incomingColor = incomingLightAmount * _LightColor0;
			half4 outgoingColor = incomingColor * half4(s.Albedo, s.Alpha); 
			
			return outgoingColor;
	}
	
	sampler2D _toonWarpTexture;
	
	half4 LightingCustomHalfLambertToonWarp(SurfaceOutput s, half3 lightDir, half atten) {
			half3 normalDir = normalize(s.Normal);
			
			half halfLambert = pow(0.5 * (dot(normalDir, lightDir) + 1), _halfLambertPower);
			half4 warpedHalfLambert = 2 * tex2D(_toonWarpTexture, half2(halfLambert,0));
			
			half4 incomingLightAmount =  warpedHalfLambert * atten;
			half4 incomingColor = incomingLightAmount * _LightColor0;
			half4 outgoingColor = incomingColor * half4(s.Albedo, s.Alpha); 
			
			return outgoingColor;
	}
		


		
		half4 LightingCustomPhong (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
			//"Normale" wieder auf volle Länge bringen =)
			half3 normalDir = normalize(s.Normal);
			
			//Lambert lighting model
			half incomingLightAmount = max(0, dot(normalDir, lightDir)) * atten;
			
			//Phong lighting model
			
			//Nochmal tricky: die "reflect" Funktion erwartet einen Vektor der von
			//der Lichtquelle Richtung Oberflächenpunkt zeigt, während
			//lightDir vom Oberflächenpunkt Richtung Lichtquelle zeigt.
			//Deshalb drehen wir das Vorzeichnen.
			half3 incidentLight = -lightDir;
			
			//Jetzt errechnet uns "reflect" den gewünschten, an der Normalen
			//reflektierten Vektor.
			half3 reflectedLightDir = reflect(incidentLight, normalDir);
			
			//Measure how much the reflected view vector is pointing into the direction
			//the light is. The nearer this value is to one, the greater is the specular
			//reflection, because the surface reflects more light near total reflection
			//angles.
			half4 specularAmount = pow(max(0, dot(viewDir, reflectedLightDir)), s.Gloss) * s.Specular;
			
			//At angles close to total reflection more light is reflected, thus
			//the diffuse amount and specular amount must be added.
			half4 reflectedLightAmount = saturate(incomingLightAmount + specularAmount);
			
			//Interaction of actual incoming radiation with surface albedo
			half4 outgoingColor = reflectedLightAmount * _LightColor0 * half4(s.Albedo, s.Alpha); 
			
			return outgoingColor;
		}
		
		half rimPower;
		
		half4 LightingPhongRim(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
		
			half3 normalDir = normalize(s.Normal);
			half incomingLightAmount = max(0, dot(normalDir, lightDir)) * atten;
			
			half3 incidentLight = -lightDir;
			half3 reflectedLightDir = reflect(incidentLight, normalDir);
			half4 specularAmount = pow(max(0, dot(viewDir, reflectedLightDir)), s.Gloss) * s.Specular;
			half4 reflectedLightAmount = saturate(incomingLightAmount + specularAmount);
			
			half4 phongColor = reflectedLightAmount * _LightColor0 * half4(s.Albedo, s.Alpha); 
			
			//Rim lighting
			
			half alignmentToView = max(0, dot(viewDir, normalDir));
			half silhouetteAmount = 1.0 - alignmentToView;
			
			return saturate(phongColor + (pow(silhouetteAmount, rimPower)) * atten * _LightColor0);  
			
		}
			
			
		
		
