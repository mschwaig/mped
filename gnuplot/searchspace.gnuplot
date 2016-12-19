set term pdfcairo size 8in,6in

set xlabel "Cayley distance to solution"
set ylabel "distance for mapping"

unset key
set tic scale 0

# Color runs from white to green
# set palette rgbformula -7,2,-7
set cbrange [0.1:2000]
set cblabel "Score"
set logscale cb

# unset cbtics

set xrange [0.0:7]
set yrange [0:124]

set xtics offset -8
set xtics 1,1,7

set view map
set pm3d interpolate 1,1

set output "searchspace_8_128_0corr.pdf"

splot 'searchspace_8_128_0corr.dat' matrix with pm3d


set output "searchspace_8_128_1corr.pdf"

splot 'searchspace_8_128_1corr.dat' matrix with pm3d


set term pngcairo size 1280,960

set output "searchspace_8_128_0corr.png"

splot 'searchspace_8_128_0corr.dat' matrix with pm3d

set output "searchspace_8_128_1corr.png"

splot 'searchspace_8_128_1corr.dat' matrix with pm3d
