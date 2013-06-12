using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Research.Kinect.Nui;
using System.IO;

namespace WpfApplication1
{
    public class fileWriter : globalVars
    {

        private System.IO.StreamWriter file;
        private bool openedStream;
        String path, fname;
        public fileWriter(String path , String fname = "kinectSkeleData.txt")
        {
            this.openedStream = false;
            this.path = path;
            this.fname = fname;
       
        }
        private void openStream()
        {        
            if (this.path == null)
                this.path = "C:\\Users\\workshop\\Desktop\\";
            this.fname = sanityChk(this.path, this.fname.Split('.')[0], "." + this.fname.Split('.')[1] );
            
            // Configure the message box to be displayed 
            
            this.file = new System.IO.StreamWriter(this.fname);
        }
        private String sanityChk(String a, String b, String c){
            String temp;
            temp = b;

            for (int i = 0;i<=10 ; ++i)
            {
                if (!File.Exists(a + temp + c))
                    return (a + temp + c);
                temp = b + i;

            } 
            return (a + temp + c);
        }

        public bool writeLog(SkeletonData sdata)
        {
            if (this.openedStream == false)
            {
                openStream();
                this.openedStream = true;
            }

            //JointID id;       
            List<Joint> l = new List<Joint>();
 
            foreach (JointID id in jid)
            {
                l.Add(sdata.Joints[id]); // the point of making this a list of joinid is we can later do it by the BVH like hierarchy as well 
                file.Write(sdata.Joints[id].Position.X.ToString("F3") + " " + sdata.Joints[id].Position.Y.ToString("F3") + " " + sdata.Joints[id].Position.Z.ToString("F3") + ", ");

            }
            file.WriteLine(" ");
            
            return true;
        }

        public bool writeFeatures(_feature f)
        {
            if (this.openedStream == false)
            {
                openStream();
                this.openedStream = true;
            }

            file.WriteLine(f.head.maxx.x + " " + f.head.maxx.z + " " + f.head.maxx.z + "," + f.head.maxy.x + " " + f.head.maxy.z + " " + f.head.maxy.z + "," + f.head.maxz.x + " " + f.head.maxz.z + " " + f.head.maxz.z);
            file.WriteLine(f.head.minx.x + " " + f.head.minx.z + " " + f.head.minx.z + "," + f.head.miny.x + " " + f.head.miny.z + " " + f.head.miny.z + "," + f.head.minz.x + " " + f.head.minz.z + " " + f.head.minz.z);

            file.WriteLine(f.lShoulder.maxx.x + " " + f.lShoulder.maxx.z + " " + f.lShoulder.maxx.z + "," + f.lShoulder.maxy.x + " " + f.lShoulder.maxy.z + " " + f.lShoulder.maxy.z + "," + f.lShoulder.maxz.x + " " + f.lShoulder.maxz.z + " " + f.lShoulder.maxz.z);
            file.WriteLine(f.lShoulder.minx.x + " " + f.lShoulder.minx.z + " " + f.lShoulder.minx.z + "," + f.lShoulder.miny.x + " " + f.lShoulder.miny.z + " " + f.lShoulder.miny.z + "," + f.lShoulder.minz.x + " " + f.lShoulder.minz.z + " " + f.lShoulder.minz.z);

            file.WriteLine(f.rShoulder.maxx.x + " " + f.rShoulder.maxx.z + " " + f.rShoulder.maxx.z + "," + f.rShoulder.maxy.x + " " + f.rShoulder.maxy.z + " " + f.rShoulder.maxy.z + "," + f.rShoulder.maxz.x + " " + f.rShoulder.maxz.z + " " + f.rShoulder.maxz.z);
            file.WriteLine(f.rShoulder.minx.x + " " + f.rShoulder.minx.z + " " + f.rShoulder.minx.z + "," + f.rShoulder.miny.x + " " + f.rShoulder.miny.z + " " + f.rShoulder.miny.z + "," + f.rShoulder.minz.x + " " + f.rShoulder.minz.z + " " + f.rShoulder.minz.z);

            file.WriteLine(f.speedMps);

            return true;
        }

        public void closeFile()
        {
            file.Close();
        }
    }
}



