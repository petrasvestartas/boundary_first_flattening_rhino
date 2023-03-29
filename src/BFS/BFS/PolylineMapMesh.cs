using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace BFS
{
    public class PolylineMapMesh : GH_Component
    {
        public PolylineMapMesh()
          : base("MapPolylines", "MapPolylines",
            "Map polylines from one mesh to another",
            "logi", "mesh")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Curve to map", GH_ParamAccess.list);
            pManager.AddGenericParameter("Mesh0", "M0", "Source Mesh", GH_ParamAccess.item);
            pManager.AddGenericParameter("Mesh1", "M1", "Target Mesh", GH_ParamAccess.item);


        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "A list of curves", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {

            ///////////////////////////////////////////////////////////////////////////////////////////////
            /// Inputs
            ///////////////////////////////////////////////////////////////////////////////////////////////
            List<Curve> curves = new List<Curve>();

            Mesh m0 = new Mesh();
            Mesh m1 = new Mesh();

            DA.GetDataList(0, curves);
            DA.GetData(1, ref m0);
            DA.GetData(2, ref m1);


            ///////////////////////////////////////////////////////////////////////////////////////////////
            /// Convert curve to polylines
            ///////////////////////////////////////////////////////////////////////////////////////////////

            List<Polyline> polylines = new List<Polyline>();
            foreach (Curve c in curves)
            {
                Polyline polyline;
                if (c.TryGetPolyline(out polyline))
                {
                    polylines.Add(polyline);
                }
            }


            ///////////////////////////////////////////////////////////////////////////////////////////////
            /// Perform mapping
            ///////////////////////////////////////////////////////////////////////////////////////////////

            List<Polyline> polylines_mapped = new List<Polyline>();
            polylines_mapped = MappedFromMeshToMesh(polylines, m0, m1);

            ///////////////////////////////////////////////////////////////////////////////////////////////
            /// Output
            ///////////////////////////////////////////////////////////////////////////////////////////////
            DA.SetDataList(0, polylines_mapped);


        }

        public List<Polyline> MappedFromMeshToMesh( List<Polyline> polylines, Mesh s, Mesh t)
        {

            List<Polyline> polylines_mapped = new List<Polyline>();

            for (int i = 0; i < polylines.Count; i++)
            {
                Polyline polyline_mapped = new Polyline(polylines[i]);


                for (int j = 0; j < polyline_mapped.Count; j++)
                {

                    Point3d pTemp = new Point3d(polyline_mapped[j]);

                    MeshPoint mp = s.ClosestMeshPoint(polyline_mapped[j], 10.01);

                   
                    if (mp == null)
                    {
                        break;
                    }

                    polyline_mapped[j] = t.PointAt(mp);



                }//for j
                polylines_mapped.Add(polyline_mapped);
            }//for i

            return polylines_mapped;

        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Resources.remap;
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("a9bf996b-570a-1581-8b14-4b46be84a967"); }
        }
    }
}