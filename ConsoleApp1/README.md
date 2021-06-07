TODO:

* naming convention: block = thing with properties, blockid (or voxel) is just an id. - be consistant with something..
* refactor position/physics? into component (YES!!! because now hacks are creeping in with booleans to enable+disable things. .. badbadbad)
* make inventory slots one draw-call by doing CPU transformations (rotate + scale + translate)
* implement simple inventory - only having 8 slots - keeping track of block # for each slot
* consider dynamic vertices vs static - so anythink that might be updated will be dynamic ? - support this in the VAOWrapper
* consider to use indexbuffers to minimize vertex data size - transferring to GPU should be faster this way i guess? - especially relevant if we have dynamic VBO
* look at the worldcomponent - architecture is wrong I believe. iterating worlds and having a world component seems wrong. Is it like the input compoennt - use a tag and see it as a shared component? 

DONE:

* attach camera to objects. So camera is attached to player.
* consider having InputComponent as a "tag"-component basically used for systems to distinguish between entities (example: player with or without control)
* BUG: skylight dark propagation doesn't work fully
* BUG: UI radiobuttons has "X" in them to begin with
* night/day time
* debug view showing #drawcalls. time% spent in update and render, #chunk rebuilds
* implement skyligt 
* smooth lighting
* implement light placement
* implement "world generation"
* convert rest of game.update to component/system
* 3D ORTHO render for inventory
* convert chunk+renderer to component/system
* convert camera to entity/system
* kill IEntity, ICanRender, ICanUpdate
* bug: cannot reproduce -> something causes camera.direction to be {NaN, NaN, NaN}

PACKAGE:
* dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true
