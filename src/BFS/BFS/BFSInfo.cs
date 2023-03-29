using System;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;

namespace BFS
{
    public class BFSInfo : GH_AssemblyInfo
    {
        public override string Name => "BFS";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "Flatten triangular meshes";

        public override Guid Id => new Guid("0B413DC1-07D2-499C-83CC-CDC904EB1271");

        //Return a string identifying you or your company.
        public override string AuthorName => "Petras Vestartas";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "petrasvestartas@gmail.com";
    }
}
