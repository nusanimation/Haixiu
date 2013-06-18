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
        public fileWriter(String path , String fname = "kinectSkeleData.csv")
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

            file.Write(f.head.maxx.x + "," + f.head.maxx.y + "," + f.head.maxx.z + "," + f.head.maxy.x + "," + f.head.maxy.y + "," + f.head.maxy.z + "," + f.head.maxz.x + "," + f.head.maxz.y + "," + f.head.maxz.z);
            file.Write(f.head.minx.x + "," + f.head.minx.y + "," + f.head.minx.z + "," + f.head.miny.x + "," + f.head.miny.y + "," + f.head.miny.z + "," + f.head.minz.x + "," + f.head.minz.y + "," + f.head.minz.z);

            file.Write(f.lShoulder.maxx.x + "," + f.lShoulder.maxx.y + "," + f.lShoulder.maxx.z + "," + f.lShoulder.maxy.x + "," + f.lShoulder.maxy.y + "," + f.lShoulder.maxy.z + "," + f.lShoulder.maxz.x + "," + f.lShoulder.maxz.y + "," + f.lShoulder.maxz.z);
            file.Write(f.lShoulder.minx.x + "," + f.lShoulder.minx.y + "," + f.lShoulder.minx.z + "," + f.lShoulder.miny.x + "," + f.lShoulder.miny.y + "," + f.lShoulder.miny.z + "," + f.lShoulder.minz.x + "," + f.lShoulder.minz.y + "," + f.lShoulder.minz.z);

            file.Write(f.rShoulder.maxx.x + "," + f.rShoulder.maxx.y + "," + f.rShoulder.maxx.z + "," + f.rShoulder.maxy.x + "," + f.rShoulder.maxy.y + "," + f.rShoulder.maxy.z + "," + f.rShoulder.maxz.x + "," + f.rShoulder.maxz.y + "," + f.rShoulder.maxz.z);
            file.Write(f.rShoulder.minx.x + "," + f.rShoulder.minx.y + "," + f.rShoulder.minx.z + "," + f.rShoulder.miny.x + "," + f.rShoulder.miny.y + "," + f.rShoulder.miny.z + "," + f.rShoulder.minz.x + "," + f.rShoulder.minz.y + "," + f.rShoulder.minz.z);

            file.Write(f.rElbow.maxx.x + "," + f.rElbow.maxx.y + "," + f.rElbow.maxx.z + "," + f.rElbow.maxy.x + "," + f.rElbow.maxy.y + "," + f.rElbow.maxy.z + "," + f.rElbow.maxz.x + "," + f.rElbow.maxz.y + "," + f.rElbow.maxz.z);
            file.Write(f.rElbow.minx.x + "," + f.rElbow.minx.y + "," + f.rElbow.minx.z + "," + f.rElbow.miny.x + "," + f.rElbow.miny.y + "," + f.rElbow.miny.z + "," + f.rElbow.minz.x + "," + f.rElbow.minz.y + "," + f.rElbow.minz.z);

            file.Write(f.lElbow.maxx.x + "," + f.lElbow.maxx.y + "," + f.lElbow.maxx.z + "," + f.lElbow.maxy.x + "," + f.lElbow.maxy.y + "," + f.lElbow.maxy.z + "," + f.lElbow.maxz.x + "," + f.lElbow.maxz.y + "," + f.lElbow.maxz.z);
            file.Write(f.lElbow.minx.x + "," + f.lElbow.minx.y + "," + f.lElbow.minx.z + "," + f.lElbow.miny.x + "," + f.lElbow.miny.y + "," + f.lElbow.miny.z + "," + f.lElbow.minz.x + "," + f.lElbow.minz.y + "," + f.lElbow.minz.z);

            file.Write(f.rHand.maxx.x + "," + f.rHand.maxx.y + "," + f.rHand.maxx.z + "," + f.rHand.maxy.x + "," + f.rHand.maxy.y + "," + f.rHand.maxy.z + "," + f.rHand.maxz.x + "," + f.rHand.maxz.y + "," + f.rHand.maxz.z);
            file.Write(f.rHand.minx.x + "," + f.rHand.minx.y + "," + f.rHand.minx.z + "," + f.rHand.miny.x + "," + f.rHand.miny.y + "," + f.rHand.miny.z + "," + f.rHand.minz.x + "," + f.rHand.minz.y + "," + f.rHand.minz.z);

            file.Write(f.lHand.maxx.x + "," + f.lHand.maxx.y + "," + f.lHand.maxx.z + "," + f.lHand.maxy.x + "," + f.lHand.maxy.y + "," + f.lHand.maxy.z + "," + f.lHand.maxz.x + "," + f.lHand.maxz.y + "," + f.lHand.maxz.z);
            file.Write(f.lHand.minx.x + "," + f.lHand.minx.y + "," + f.lHand.minx.z + "," + f.lHand.miny.x + "," + f.lHand.miny.y + "," + f.lHand.miny.z + "," + f.lHand.minz.x + "," + f.lHand.minz.y + "," + f.lHand.minz.z);


            file.Write(f.rWrist.maxx.x + "," + f.rWrist.maxx.y + "," + f.rWrist.maxx.z + "," + f.rWrist.maxy.x + "," + f.rWrist.maxy.y + "," + f.rWrist.maxy.z + "," + f.rWrist.maxz.x + "," + f.rWrist.maxz.y + "," + f.rWrist.maxz.z);
            file.Write(f.rWrist.minx.x + "," + f.rWrist.minx.y + "," + f.rWrist.minx.z + "," + f.rWrist.miny.x + "," + f.rWrist.miny.y + "," + f.rWrist.miny.z + "," + f.rWrist.minz.x + "," + f.rWrist.minz.y + "," + f.rWrist.minz.z);

            file.Write(f.lWrist.maxx.x + "," + f.lWrist.maxx.y + "," + f.lWrist.maxx.z + "," + f.lWrist.maxy.x + "," + f.lWrist.maxy.y + "," + f.lWrist.maxy.z + "," + f.lWrist.maxz.x + "," + f.lWrist.maxz.y + "," + f.lWrist.maxz.z);
            file.Write(f.lWrist.minx.x + "," + f.lWrist.minx.y + "," + f.lWrist.minx.z + "," + f.lWrist.miny.x + "," + f.lWrist.miny.y + "," + f.lWrist.miny.z + "," + f.lWrist.minz.x + "," + f.lWrist.minz.y + "," + f.lWrist.minz.z);

            file.Write(f.speedMps);

            return true;
        }

        public void closeFile()
        {
            file.Close();
        }
    }
}



