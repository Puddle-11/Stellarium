This project provides modular Newtonian gravity, with some cool features like adaptive orbits and customizable systems.

Setting up a System
	(Optional) Create an empty gameobject.
		 Add the simulationvariables script to this gameobject this script is used to change the global timescale aswell as the global gravity force.
		 


	Create an empty gameobject
		Add the SystemManager to this gameobject
			If you do not have a simulationvariables script, useGlobalGravity wont work so you will have to set the local gravity
			There are three different types of systems, auto, manual, and single body. 
				Singlebody: only adds the first object to the simulation, which means all other objects will only be attracted to the primary object
				Auto: Automatically adds all objects to the simulation, this can be costly with a big O of O((n-1)n)
				Manual: Manual will not add any gameobjects to the simulation, you will have to manually add the transforms, the gravity scripts, and manually set the mass and radius.
			Systems are automatically set to Singlebody. this is best for solar systems however might not be what you are looking for.
			SystemSize controlls how far objects can move away from the center of the system before they get pulled back.
			SystemSafeForce controlls how quickly these objects are pulled back in
			RepulsionDistance controls how close objects can get before being pushed away from one another (this is useful if you dont want any of your planets to "stick" to one another)
			RepulsionForce controls quickly they are pushed away


		Once you have set up your System Manager begin adding children to this system manager
			There is a prefab attached in the package but the children really only need a rigidbody and a gravity script
			These children will be apart of the system. if you have selected manual, the children will only experience a gravitational pull and not exert one.
			
		Gravity
			Set the objects mass in the rigidbody component, (only if system is on auto) this will effect how strong its field is
			StartVelocitytype controls how the gameobejct moves on start
			StartvelocityType has 3 options: Auto, Manual, None. 
				Auto: (Only for singlebody Systems) automatically adjusts the initial start velocity to create a desirable orbit around the primary object. please note, the startVelocity field will be normalized, the StartVelocity field is used to choose the axis the body moves along.
				Manual: applies the Velocity entered by the player directly.
				None: Applies no start velocity.

			
			
	
	