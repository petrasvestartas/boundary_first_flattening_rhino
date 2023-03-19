using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

// for mac add icons follow:
// https://discourse.mcneel.com/t/simple-instructions-for-adding-an-icon-to-a-mac-c-grasshopper-component/91638/8

namespace BoundaryFirstFlatterningMac
{
    public class BoundaryFirstFlatenning : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public BoundaryFirstFlatenning()
          : base("BoundaryFirstFlatenning", "BoundaryFirstFlatenning",
            "Boundary First Flatenning for mac",
            "logi", "mesh")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Mesh", "M", "Mesh, that must be triangulated", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Type", "T", "Unwrapping type, Flatten = 0 or other, Map to sphere = 1, Map to circle = 2", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("NCones", "N", "Number of cones, if set to 0, automatic number is computed", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("NormalizeUVs", "UV", "Normalize UVs, so that they are in the range [0,1] x [0,1]", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("VertexOrder", "V", "Keep the original mesh vertex order, it can be slow, if set to true", GH_ParamAccess.item, false);
            //pManager.AddTextParameter("Path", "P", "Path of the BFF folder, only needed if component gives you error message", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            //pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Mesh3D", "M3D", "Mesh3D", GH_ParamAccess.item);
            pManager.AddGenericParameter("Mesh2D", "M2D", "Mesh2D", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            ///////////////////////////////////////////////////////////////////////////////////////////////
            /// Inputs
            ///////////////////////////////////////////////////////////////////////////////////////////////
            Mesh mesh = new Mesh();
            DA.GetData(0, ref mesh);

            int flatten_type = 0;
            DA.GetData(1, ref flatten_type);

            int number_of_cones = 0;
            DA.GetData(2, ref number_of_cones);

            bool normalize_uvs = false;
            DA.GetData(3, ref normalize_uvs);

            bool vertex_order = false;
            DA.GetData(4, ref vertex_order);

            string user_path = "";
            //DA.GetData(5, ref user_path);

            bool win_or_mac = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

            ///////////////////////////////////////////////////////////////////////////////////////////////
            /// Triangulate mesh and check if it is valid
            ///////////////////////////////////////////////////////////////////////////////////////////////

            // copy and triangulate mesh
            Mesh mesh_cleaned = mesh.DuplicateMesh();
            mesh_cleaned.Faces.ConvertQuadsToTriangles();

            // check validity of the mesh
            bool isOriented;
            bool hasBoundary;
            if ((mesh_cleaned.IsValid && mesh_cleaned.IsManifold(true, out isOriented, out hasBoundary)) == false)
                return;


            ///////////////////////////////////////////////////////////////////////////////////////////////
            /// Get local path
            ///////////////////////////////////////////////////////////////////////////////////////////////


            // get path of the current .gha file
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(ObjParser.Obj)).Location;
            string assemblyPath = System.IO.Path.GetDirectoryName(assembly);

            // if user gave the input, replace the previous path
            if (user_path != null)
                if (user_path != "")
                    assemblyPath = user_path;

            // depending if OS is math or windows concatenatte the path
            // https://stackoverflow.com/questions/15452651/start-an-external-process-on-mac-with-c-sharp

            string temp_filepath;
            if (win_or_mac)
                temp_filepath  = assemblyPath + @"\windows\in.obj";
            else
                temp_filepath = assemblyPath + @"/MacOS/in.obj";


            // Rhino.RhinoApp.WriteLine(temp_filepath);

            // delete input obj file that will be replace by the newly generated one
            if (System.IO.File.Exists(temp_filepath))
                System.IO.File.Delete(temp_filepath);


            ///////////////////////////////////////////////////////////////////////////////////////////////
            /// Obj Parser
            ///////////////////////////////////////////////////////////////////////////////////////////////
            
            
            // convert mesh vertices and faces to a string array
            string[] verticas_and_faces = new string[mesh_cleaned.Vertices.Count+ mesh_cleaned.Faces.Count];

            for (int i = 0; i < mesh_cleaned.Vertices.Count; i++)
                verticas_and_faces[i] = "v" + " " + mesh_cleaned.Vertices[i].X.ToString() + " " + mesh_cleaned.Vertices[i].Y.ToString() + " " + mesh_cleaned.Vertices[i].Z.ToString();

            for (int i = 0; i < mesh_cleaned.Faces.Count; i++)
                verticas_and_faces[i + mesh_cleaned.Vertices.Count] = "f" + " " + (mesh_cleaned.Faces[i].A+1).ToString() + " " + (mesh_cleaned.Faces[i].B + 1).ToString() + " " + (mesh_cleaned.Faces[i].C + 1).ToString();

            // write obj file
            var objInput = new ObjParser.Obj();
            objInput.LoadObj(verticas_and_faces);
            string[] headers = new string[] { "ObjParser" };
            objInput.WriteObjFile(temp_filepath, headers);

            ///////////////////////////////////////////////////////////////////////////////////////////////
            /// Run the executable
            ///////////////////////////////////////////////////////////////////////////////////////////////

            // file path
            string filename = win_or_mac ? assemblyPath + @"\windows\bff-command-line" : assemblyPath + @"/MacOS/bff-command-line";
            string workingDirectory = win_or_mac ? assemblyPath + @"\windows\" : assemblyPath + @"/MacOS/";


            // arguments
            string arguments = "in.obj out.obj";

            //--flattenToDisk
            if (flatten_type==1)
                arguments += " --mapToSphere";

            if (flatten_type==2)
                arguments += " --flattenToDisk";

            //--nCones=5
            if (number_of_cones > 0)
                arguments += " --nCones=" + Math.Abs(number_of_cones).ToString();

            //--normalizeUVs
            if (normalize_uvs)
            {
                arguments += " --normalizeUVs";
            }

            // run the process (executable)
            var proc = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filename,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        WorkingDirectory = workingDirectory
                    }
                };

            proc.Start();
            proc.WaitForExit();

            ///////////////////////////////////////////////////////////////////////////////////////////////
            /// Read the output: out.obj file via "ObjParser"
            ///////////////////////////////////////////////////////////////////////////////////////////////

            // Initialize the ObjParser
            var obj = new ObjParser.Obj();

            // Read Wavefront OBJ file
            if(win_or_mac)
                obj.LoadObj(assemblyPath + @"/windows/out.obj");
            else
                obj.LoadObj(assemblyPath + @"/MacOS/out.obj");


            // conver the ObjParser data to Rhino mesh
            Mesh mesh_3d = new Mesh();
            Mesh mesh_2d = new Mesh();
            List<Point3d> points_3d = new List<Point3d>(mesh_cleaned.Vertices.Count);
            List<Point3d> points_2d = new List<Point3d>(mesh_cleaned.Vertices.Count);
            List<MeshFace> faces_3d = new List<MeshFace>(mesh_cleaned.Faces.Count);
            List<MeshFace> faces_2d = new List<MeshFace>(mesh_cleaned.Faces.Count);

            // add vertices of a 3d mesh
            foreach (ObjParser.Types.Vertex v in obj.VertexList)
                mesh_3d.Vertices.Add(new Point3d(v.X, v.Y, v.Z));

            // add vertices of a 2d mesh as textures coordinates, incase there is a sphere perform the mapping

            Sphere s = new Sphere(Plane.WorldXY, 1);
            NurbsSurface ns = s.ToNurbsSurface();
            ns.SetDomain(0, new Interval(0, 1));
            ns.SetDomain(1, new Interval(0, 1));

            foreach (ObjParser.Types.TextureVertex v in obj.TextureList)
                if (flatten_type==1)
                {
                    Point3d p = new Point3d(0, 0, 0);
                    p = ns.PointAt(v.X, v.Y);
                    mesh_2d.Vertices.Add(p);//0
                }
                else
                {
                    mesh_2d.Vertices.Add(new Point3d(v.X, v.Y, 0));//0
                }

            // add faces
            foreach (ObjParser.Types.Face f in obj.FaceList)
            {

                string[] lineData = f.ToString().Split(' ');
                string[] v0 = lineData[1].Split('/');
                string[] v1 = lineData[2].Split('/');
                string[] v2 = lineData[3].Split('/');

                MeshFace mf_3d = new MeshFace(Convert.ToInt32(v0[0]) - 1, Convert.ToInt32(v1[0]) - 1, Convert.ToInt32(v2[0]) - 1);
                MeshFace mf_2d = new MeshFace(Convert.ToInt32(v0[1]) - 1, Convert.ToInt32(v1[1]) - 1, Convert.ToInt32(v2[1]) - 1);
                mesh_3d.Faces.AddFace(mf_3d);
                mesh_2d.Faces.AddFace(mf_2d);
            }

            // recompute normals
            mesh_2d.RebuildNormals();
            mesh_3d.RebuildNormals();


            // move the 3d mesh to the original position, this mismatch only exists for windows
            if (win_or_mac)
            {
                
                //BoundingBox bbox = mesh_2d.GetBoundingBox(false);
                Vector3d vec = mesh_cleaned.Vertices[0] - mesh_3d.Vertices[0];
                mesh_3d.Translate(vec);
            }


            // remap vertices
            // map point from 3d to 2d
            if (vertex_order)
            {

               

                // get vertex correspondence between input mesh and bff mesh

                Point3d[] vertices = new Point3d[mesh_cleaned.Vertices.Count];
                bool success = true;
                for (int i = 0; i < mesh_cleaned.Vertices.Count; i++)
                {
                    MeshPoint mp = mesh_3d.ClosestMeshPoint(mesh_cleaned.Vertices[i], 1.0);
                    if (mp == null)
                    {
                        success = false;
                        break;
                    }

                    Point3d new_vertex_point = mesh_2d.PointAt(mp);
                    vertices[i] = new_vertex_point;

                }

                // replace the output 2d mesh with the closest points
                if (success)
                {
                    Mesh mesh_2d_remapped = new Mesh();
                    mesh_2d_remapped.Vertices.AddVertices(vertices);
                    mesh_2d_remapped.Faces.AddFaces(mesh_3d.Faces);
                    mesh_2d = mesh_2d_remapped;
                }


            }

            // output
            DA.SetData(0, mesh_3d);
            DA.SetData(1, mesh_2d);





            

        }


        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Resources.bff;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("477EE1B4-87C2-43C9-A92C-70B52D6DD883");
    }
}
