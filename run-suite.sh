#!/bin/bash

experiment=32
time_out=$((10*1))  # minutes
stime_out=$((10*1 + 1))  # minutes
slurm_timeout=$(printf "%d-%02d:%02d:%02d" $(($stime_out/1440)) $(($stime_out%1440/60)) $(($stime_out%60)) 0)
partition='rome'

home='/nfs/home/student.aau.dk/rhha19'
binary="$home/CatSynth/resources/catsynth/Cat-Synth"
job_script_dir="${home}/YAFMS-MC/results/${experiment}"
mkdir -p $job_script_dir

echo "Constructing experiment run $experiment on $binary with timeout $time_out"

systems=("S2" "S3")
instances=("1" "2" "3" "4" "5" "6" "7" "8" "9" "10" "11" "12" "13" "14" "15" "16" "17" "18" "19" "20" "21" "22" "23" "24")
guided_algorithms=("catstar")
heuristics=("RPT-T" "RPT" "MRPT" "MRPT-T")
unguided_algorithms=("depth-first")

create_job_script() {
    local problem=$1
    local job_script=$2
    local binary_cmd=$3

    echo "Creating job for $problem"
    cat <<EOT > $job_script
#!/bin/bash
#SBATCH --mail-type=FAIL # Type of email notification- BEGIN,END,FAIL,ALL
#SBATCH --mail-user=rhha19@cs.aau.dk
#SBATCH --job-name=YAFMS-${problem}
#SBATCH --cpus-per-task=2  # Number of cores to use.
#SBATCH --mem=32G          # Memory limit (can be overwritten by sbatch command, if needed)
#SBATCH --output=${job_script_dir}/${problem}.out
#SBATCH --error=${job_script_dir}/${problem}.err
#SBATCH --time=${slurm_timeout}
#SBATCH --partition=${partition}
$binary_cmd
EOT
    chmod +x $job_script
    sbatch $job_script
}
for system in "${systems[@]}"; do
    for instance in "${instances[@]}"; do
        for alg in "${guided_algorithms[@]}"; do
            for h in "${heuristics[@]}"; do
                problem="${system}-${instance}-${alg}-${h}"
                job_script="$job_script_dir/sbatch-${problem}.sh"
                binary_cmd="\"$binary\" synth \"$system\" \"$instance\" \"$alg\" \"$time_out\" --heuristic \"$h\""
                create_job_script "$problem" "$job_script" "$binary_cmd"
            done
        done

        for alg in "${unguided_algorithms[@]}"; do
            problem="${system}-${instance}-${alg}"
            job_script="$job_script_dir/sbatch-${problem}.sh"
            binary_cmd="\"$binary\" synth \"$system\" \"$instance\" \"$alg\" \"$time_out\""
            create_job_script "$problem" "$job_script" "$binary_cmd"
        done
    done
done
