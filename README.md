# 🦏 BOUNDARY FIRST FLATTENING FOR RHINO 🦏

## Installation

* Download the full repostitory by cloning the github repo or downloading the zip file (click the top right green button).
* Go to the "build" folder: https://github.com/petrasvestartas/boundary_first_flattening_rhino/tree/main/build/BoundaryFirstFlattenning
* Copy the "BoundaryFirstFlattenning" folder to the Grasshopper libraries folder
* incase the files are blocked, unblock them by right click and properties (the usual Grasshopper library installation method)
* special case for MAC. STEP1: go to grasshopper directories folder where you installed the library. STEP2: open terminal and go to "boundary_first_flatenning" directory: type "cd" and drag and drop folder "boundary_first_flatenning". STEP3: run this command in the terminal: "chmod -R 777 MacOS"

## Testing

This is a cross platform version that works both on windows and mac os.

Grasshopper examples files are located in "test" folder.

![image](https://user-images.githubusercontent.com/18013985/226210126-99ac20ce-2c88-4d0b-9175-9b62cdf4a176.png)
<img width="1365" alt="Screenshot 2023-03-19 at 22 29 33" src="https://user-images.githubusercontent.com/18013985/226210877-42c35128-e0f1-4049-8eec-20a308857d13.png">


## MILESTONES

- [x] timeline 2023-02-23 - 2023-03-23
- [x] code development for BFF Rhino3D GH
- [x] testing
- [ ] revision
- [x] final release

## RHINO MAC

Development of a MacOS-compatible Boundary First Flattening (“BFF”) component for Grasshopper 3D (which is to run in Rhino 7).

MacOS-compatible version of the BFF component for Grasshopper 3D (running in Rhino 7). BFF is a low-distortion conformal parameterization technique which is described in more detail in ACM Transactions on Graphics, Volume 37, Issue 1, Article No: 5, pp 1–14 and https://github.com/GeometryCollective/boundary-first-flattening .



**INPUT**
* Mesh: Valid triangulated Mesh
* Type: Integer 0 - flatten, 1 - map to sphere (for closed meshes only, without holes meaning now donuts), 2 - map to disk (for open meshes only)
* Number of cones: by default the input is 0, so the algorithm decides how many points are needed, otherwise the user can decided how many cones must be placed.
* UV - the flat geometry is mapped to unit domain
* Vertex Order - by default the flatenned mesh has a different vertex order than the 3d mesh that would work just fine for curve remapping. If you need to keep the 

**OUTPUT**
* 3D of Mesh
* 2D of Mesh (BFF)

The MacOS-compatible version should have the same inputs, outputs and functionality as NGon BFF windows version. The component should  successfully on MacOS in Rhino 7 and is built as a C# component.

## QUESTIONS | ISSUES

Any question or issues must be added under Issues tab in Git-hub to reduce email communcation and keep data in one location.
