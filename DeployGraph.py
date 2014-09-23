import glob
import shutil

dest_dir = "..\Matlab\Bin"

for file in glob.glob(r'.\Marv.Common.Graph\bin\Debug\*.dll'):
    print file                                                                                                                                        
    shutil.copy(file, dest_dir)
