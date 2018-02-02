## Electrostatics VR

This is my first Unity3D VR project. It displays the electric field due to one or more charged particles and it is designed to run on Google Cardboard. It can also be extended to Daydream if we use the GvrController class, the GvrControllerPointer prefab and other assets from the GoogleVR library.

When loaded into a mobile device, the application shows a room with one red sphere that represents a positive point charge floating in space. There are blue arrows showing the direction of the electric field at a certain point, and also hinting at the magnitude of the electric field as the arrows change sizes. The user may gaze at the charged sphere, click on it and flip its value from positive to negative or viceversa. The user can also point at any floor tile and click on it to create another charge right above that tile. We can increase the height of the charged sphere by clicking on the same tile again, as many times as necessary.

<img src="/Pictures/Dipole.PNG" alt="Pattern" style="width:50px;height:30px"/>

### Main custom assets

The main assets we have created in this project are:

1. 3D Objects (prefabs):
	
	* Black Tile: It is a cube that has been shaped into a tile and has the ChargeCreator.cs script attached to it (more info below). It also has an Event Trigger attached, a native Unity feature, so that when the user gazes at the tile or clicks on the tile we can call the public methods in ChargeCreator.cs
		
	* Charged Sphere: It is a small sphere representing a point charge. It has ChargeProperties.cs attached to it with the only purpose of holding the value of the charge in Coulombs. It also has the ChargeModifier.cs script attached, and also an Event Trigger so that we can call any public method in ChargeModifier.cs when the user gazes at or clicks on the charged sphere.
		
	* FieldArrow: This is a prefab created from a cylinder, a pyramid and a tiny square plane to form a 3D arrow that represents the electric field at a certain point. The ElectricFieldCreator.cs script is in charge of creating instances of this object and change them according to the magnitude and direction of the electric field computed at a certain point.

2. Scripts:
	
	* ChargeCreator: This script has two public methods that are called by the Event Trigger attached to the Black Tile prefab: HighlightTile and MoveChargeUp. When the user gazes at a tile or looks away from it, the method HighlightTile gets called. When the user clicks on a tile, the event trigger will call the method MoveChargeUp. This method creates a charge on top of the selected tile or moves the charge up if there is one already there.
		
	* ChargeModifier: This script has two public methods that are called by the Event Trigger attached to the Charged Sphere prefab: SetGazedAt and FlipChargeValue. When the user gazes at a Charged Sphere or looks away from it, the method SetGazedAt is called to change the material of the sphere. This creates a "highlighting" effect. When the user clicks on the Charged Sphere, the method FlipChargeValue is called, which changes the sign of the charge value.
		
	* ElectricFieldCreator: This script computes the electric field's x,y,z components for certain points in space, caused every charge in the scene. The script will clone the FieldArrow prefab, resize it and re-direction it according to the electric field components at every evaluated point. The script is attached to an empty game object in the scene with the name "Electric Field Creator".
		
	* FloorCreator: Simple script that runs once and clones the Black Tile prefab several times (with some predetermined spacing) to create a tiled floor. The script is attached to an empty game object in the scene with the name "Floor Creator".
		
	* ChargeProperties: It has no methods. Its only purpose is to hold a charge value when attached to a Charged Sphere prefab.
		
	* ElectricFieldComponents: It has no methods. Its only purpose is to hold Ex, Ey, Ez values when attached to a FieldArrow prefab.
		
3. Materials:
	
	All materials are in the "My Materials" folder and their only purpose as of now is to give color to 3D objects. Materials can be attached to 3D objects either statically, by dragging the material onto the object, or dynamically from a C# script (example in ChargeModifier.cs, method SetGazedAt).

### Scenes

<img src="/Pictures/MoreCharges.PNG" alt="Pattern" style="width:50px;height:30px"/>

Right now there is only one scene in the Unity project. Several other scenes have existed before, but they have been deleted and all of their features have been combined into one: "Dipole Scene (interactive)". The scene contains:

1. GvrEventSystem: It holds the GvrPointerInputModule class, necessary to make any Google VR interaction work. No changes should be made to this object.(https://developers.google.com/vr/unity/reference/class/gvr-pointer-input-module#class_gvr_pointer_input_module)

2. Main Camera: Move it if you need to change the position of the user. Rotate it if you want to change their *starting* point of view when the application is launched (anyway the user can rotate their head as they please once they start the application). This camera object in special (in our scene) has the GvrPointerPhysicsRaycaster and the GvrReticlePointer attached to it so that the user can point at and click on 3D objects.

3. Change Indicator: Empty object, but very important....

