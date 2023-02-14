

# Command Line Interface

Current version exmploys the compiled BFF version by calling the application cia .NET
To run the command line interface, simply navigate into the directory containing the executable bff-command-line and type

```
./bff-command-line in.obj out.obj
```

where in.obj is the mesh you want to flatten, and out.obj is the same mesh with the output UV coordinates.

Optional flags:
```
--nCones=N_CONES 
--normalizeUVs
--mapToSphere
--flattenToDisk
```

--nCones=N_CONES Use the specified number of cone singularities to reduce area distortion (these are chosen automatically)
--normalizeUVs Scale all UVs so that they are in the range [0,1] x [0,1].
--mapToSphere For a genus-0 surface (no holes, handles, or boundary), computes a flattening over the unit sphere rather than the plane. (See below for more detail.)
--flattenToDisk For a topological disk, maps to the unit circular disk. (See below for more detail.)

# Compilation From Source in C++


## Clone the original repository
```
git clone https://github.com/GeometryCollective/boundary-first-flattening.git
cd boundary-first-flattening && git submodule update --init --recursive
mkdir build && cd build && cmake ..
make -j 4
```

## Dependencies

https://github.com/GeometryCollective/boundary-first-flattening/tree/master/deps
