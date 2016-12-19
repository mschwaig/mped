unset key
set xtics rotate out
set style data histogram
set style fill solid border

set style histogram errorbars linewidth 1
set style fill solid 0.3
set bars front
set xtics noenhanced
set autoscale y

alphabet_sizes="8 12 16"
correlations="0 0.5 1"

do for [alphabet_size in alphabet_sizes] {

do for [correlation in correlations] {

    set ylabel "MPED (lower is better)"
    set xlabel "evaluations"
    
    set key autotitle columnheader outside right

    file_name = sprintf("linecomp_%s_%s", alphabet_size, correlation)
    input_file = sprintf("%s.dat", file_name)
        
    stats input_file nooutput
    
    set term pdfcairo size 8in,6in
    set output sprintf("%s.pdf", file_name)
    
    plot for [i=0:STATS_blocks-2]  input_file using 1:2 index i with linespoints

    set term pngcairo size 1280,960

    set output sprintf("%s.png", file_name)

    plot for [i=0:STATS_blocks-2]  input_file using 1:2 index i with linespoints
  }
}