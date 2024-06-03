import itertools
import os
import sys
import pandas as pd
import matplotlib.pyplot as plt
import numpy as np


def create_subdir(parent, dir_name:str)->str:
    append = '' if dir_name.endswith('/') else '/'

    out_directory = parent + dir_name + append
    if not os.path.exists(out_directory):
        os.makedirs(out_directory)
    return out_directory

    

def rename_fields(combined: pd.DataFrame):
        combined.loc[combined.algorithm == 'Cat-DFS-100-RPT-T', 'algorithm'] = 'Cat-DFS-1-RPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-125-RPT-T', 'algorithm'] = 'Cat-DFS-1.25-RPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-150-RPT-T', 'algorithm'] = 'Cat-DFS-1.5-RPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-175-RPT-T', 'algorithm'] = 'Cat-DFS-1.75-RPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-200-RPT-T', 'algorithm'] = 'Cat-DFS-2-RPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-225-RPT-T', 'algorithm'] = 'Cat-DFS-2.25-RPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-250-RPT-T', 'algorithm'] = 'Cat-DFS-2.5-RPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-275-RPT-T', 'algorithm'] = 'Cat-DFS-2.75-RPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-300-RPT-T', 'algorithm'] = 'Cat-DFS-3-RPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-350-RPT-T', 'algorithm'] = 'Cat-DFS-3.5-RPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-400-RPT-T', 'algorithm'] = 'Cat-DFS-4-RPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-450-RPT-T', 'algorithm'] = 'Cat-DFS-4.5-RPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-500-RPT-T', 'algorithm'] = 'Cat-DFS-5-RPT-T'


        combined.loc[combined.algorithm == 'Cat-DFS-1-RPT-T', 'algorithm'] = 'RPT-T, E = 1.00'
        combined.loc[combined.algorithm == 'Cat-DFS-1.25-RPT-T', 'algorithm'] = 'RPT-T, E = 1.25'
        combined.loc[combined.algorithm == 'Cat-DFS-1.5-RPT-T', 'algorithm'] = 'RPT-T, E = 1.50'
        combined.loc[combined.algorithm == 'Cat-DFS-1.75-RPT-T', 'algorithm'] = 'RPT-T, E = 1.75'
        combined.loc[combined.algorithm == 'Cat-DFS-2-RPT-T', 'algorithm'] = 'RPT-T, E = 2.00'
        combined.loc[combined.algorithm == 'Cat-DFS-2.25-RPT-T', 'algorithm'] = 'RPT-T, E = 2.25'
        combined.loc[combined.algorithm == 'Cat-DFS-2.5-RPT-T', 'algorithm'] = 'RPT-T, E = 2.50'
        combined.loc[combined.algorithm == 'Cat-DFS-2.75-RPT-T', 'algorithm'] = 'RPT-T, E = 2.75'
        combined.loc[combined.algorithm == 'Cat-DFS-3-RPT-T', 'algorithm'] = 'RPT-T, E = 3.00'
        combined.loc[combined.algorithm == 'Cat-DFS-3.25-RPT-T', 'algorithm'] = 'RPT-T, E = 3.25'
        combined.loc[combined.algorithm == 'Cat-DFS-3.5-RPT-T', 'algorithm'] = 'RPT-T, E = 3.50'
        combined.loc[combined.algorithm == 'Cat-DFS-3.75-RPT-T', 'algorithm'] = 'RPT-T, E = 3.75'
        combined.loc[combined.algorithm == 'Cat-DFS-4-RPT-T', 'algorithm'] = 'RPT-T, E = 4.00'
        combined.loc[combined.algorithm == 'Cat-DFS-4.5-RPT-T', 'algorithm'] = 'RPT-T, E = 4.50'
        combined.loc[combined.algorithm == 'Cat-DFS-5-RPT-T', 'algorithm'] = 'RPT-T, E = 5.00'
        combined = combined.drop_duplicates(subset=['algorithm', 'system', 'instance'], keep='last')

        combined.loc[combined.algorithm == 'Cat-DFS-100-MRPT-T', 'algorithm'] = 'Cat-DFS-1-MRPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-125-MRPT-T', 'algorithm'] = 'Cat-DFS-1.25-MRPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-150-MRPT-T', 'algorithm'] = 'Cat-DFS-1.5-MRPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-175-MRPT-T', 'algorithm'] = 'Cat-DFS-1.75-MRPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-200-MRPT-T', 'algorithm'] = 'Cat-DFS-2-MRPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-225-MRPT-T', 'algorithm'] = 'Cat-DFS-2.25-MRPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-250-MRPT-T', 'algorithm'] = 'Cat-DFS-2.5-MRPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-275-MRPT-T', 'algorithm'] = 'Cat-DFS-2.75-MRPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-300-MRPT-T', 'algorithm'] = 'Cat-DFS-3-MRPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-350-MRPT-T', 'algorithm'] = 'Cat-DFS-3.5-MRPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-400-MRPT-T', 'algorithm'] = 'Cat-DFS-4-MRPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-450-MRPT-T', 'algorithm'] = 'Cat-DFS-4.5-MRPT-T'
        combined.loc[combined.algorithm == 'Cat-DFS-500-MRPT-T', 'algorithm'] = 'Cat-DFS-5-MRPT-T'


        combined.loc[combined.algorithm == 'Cat-DFS-1-MRPT-T', 'algorithm'] = 'MRPT-T, E = 1.00'
        combined.loc[combined.algorithm == 'Cat-DFS-1.25-MRPT-T', 'algorithm'] = 'MRPT-T, E = 1.25'
        combined.loc[combined.algorithm == 'Cat-DFS-1.5-MRPT-T', 'algorithm'] = 'MRPT-T, E = 1.50'
        combined.loc[combined.algorithm == 'Cat-DFS-1.75-MRPT-T', 'algorithm'] = 'MRPT-T, E = 1.75'
        combined.loc[combined.algorithm == 'Cat-DFS-2-MRPT-T', 'algorithm'] = 'MRPT-T, E = 2.00'
        combined.loc[combined.algorithm == 'Cat-DFS-2.25-MRPT-T', 'algorithm'] = 'MRPT-T, E = 2.25'
        combined.loc[combined.algorithm == 'Cat-DFS-2.5-MRPT-T', 'algorithm'] = 'MRPT-T, E = 2.50'
        combined.loc[combined.algorithm == 'Cat-DFS-2.75-MRPT-T', 'algorithm'] = 'MRPT-T, E = 2.75'
        combined.loc[combined.algorithm == 'Cat-DFS-3-MRPT-T', 'algorithm'] = 'MRPT-T, E = 3.00'
        combined.loc[combined.algorithm == 'Cat-DFS-3.25-MRPT-T', 'algorithm'] = 'MRPT-T, E = 3.25'
        combined.loc[combined.algorithm == 'Cat-DFS-3.5-MRPT-T', 'algorithm'] = 'MRPT-T, E = 3.50'
        combined.loc[combined.algorithm == 'Cat-DFS-3.75-MRPT-T', 'algorithm'] = 'MRPT-T, E = 3.75'
        combined.loc[combined.algorithm == 'Cat-DFS-4-MRPT-T', 'algorithm'] = 'MRPT-T, E = 4.00'
        combined.loc[combined.algorithm == 'Cat-DFS-4.5-MRPT-T', 'algorithm'] = 'MRPT-T, E = 4.50'
        combined.loc[combined.algorithm == 'Cat-DFS-5-MRPT-T', 'algorithm'] = 'MRPT-T, E = 5.00'
        combined = combined.drop_duplicates(subset=['algorithm', 'system', 'instance'], keep='last')



    
        return combined

RESULT_DIR = './results/'
def generate_line_styles(n) -> list[dict]:
    # Predefined line styles and markers
    line_styles = ['-', '--', '-.', ':', 'None', ' ', '', 'solid', 'dashed', 'dashdot', 'dotted']
    markers = ['o', 's', 'D', '^', 'v', '<', '>', 'p', '*', '+', 'x']

    values: list[dict] = []
    for style in line_styles:
        for marker in markers:
            values.append({'style': style, 'marker': marker})

    if n > len(values):
        raise ValueError("The number of requested styles exceeds the available unique combinations.")
    
    return values[0:n]

def add_to_plot(groups, field_name: str):
        styles = generate_line_styles(len(groups))
        alg_with_lines = zip(groups, styles)
        for (algorithmName, group_data), style in alg_with_lines:
            data = group_data.sort_values(field_name, ignore_index=True)
            plot_style = style['style']
            marker = style['marker']
            plt.plot(data[field_name], label=algorithmName, linestyle=plot_style, marker=marker)


def write_latex(df: pd.DataFrame, outdir, name):
    with open(outdir + name, 'w') as texFile:
        texFile.write(df.to_latex(float_format="{:.2f}".format, index=False))


def create_figure(outdir, name, ylabel, xlabel, df: pd.DataFrame):
    plt.ylabel(ylabel)
    plt.xlabel(xlabel)
    plt.legend()
    plt.savefig(outdir + name+'.png', dpi=800)
    plt.cla()
    write_latex(df, outdir, name+'.tex')

def create_initial_dataframe(data):
    header = ['algorithm', 'system', 'instance', 'time', 'conf', 'makespan', 'found_solution']
    df = pd.DataFrame(data, columns=header)
    non_integer_values = df[~df['conf'].apply(lambda x: isinstance(x, (int, float, str)) and str(x).isdigit())]
    print(non_integer_values)
    df['time'] = df['time'].astype(float)
    df['algorithm'] = df['algorithm'].astype('string')
    df['system'] = df['system'].astype('string')
    df['conf'] = df['conf'].astype(int)
    df['makespan'] = df['makespan'].astype(int)
    df['instance'] = df['instance'].astype(int)
    df['found_solution'] = df['found_solution'].astype(bool)
    return df


def create_initial_dataframe_new(data):
    header = ['algorithm', 'system', 'instance', 'time', 'conf', 'makespan', 'found_solution', 'error_text']
    df = pd.DataFrame(data, columns=header)
    non_integer_values = df[~df['conf'].apply(lambda x: isinstance(x, (int, float, str)) and str(x).isdigit())]
    print(non_integer_values)
    df['time'] = df['time'].astype(float)
    df['algorithm'] = df['algorithm'].astype('string')
    df['system'] = df['system'].astype('string')
    df['conf'] = df['conf'].astype(int)
    df['makespan'] = df['makespan'].astype(int)
    df['instance'] = df['instance'].astype(int)
    df['found_solution'] = df['found_solution'].astype(bool)
    df['error_text'] = df['error_text'].astype('string')
    return df


def getFileText(file: str) -> str:
    try:
        with open(file, 'r') as f:
            text = f.read()
        return text
    except FileNotFoundError:
        print("File not found.")
        return ""
    except Exception as e:
        print("Error:", e)
        return ""

    
def generate_heur_time_results(df: pd.DataFrame, outdir):
    filtered_df = df[~df['algorithm'].str.contains('DFS|CatDfs', case=False)]
    algorithmTime = filtered_df[['algorithm', 'time']].sort_values(['algorithm', 'time'], ignore_index=True)
    algorithmTime.to_csv(outdir + 'heuristics_time.csv', index=False, header=True)

    algorithmGroups = algorithmTime.groupby('algorithm')
    add_to_plot(field_name='time', groups=algorithmGroups)
    create_figure(df=algorithmTime, outdir=outdir, name="heuristics_time",xlabel='Index', ylabel='Time (seconds)')


def generate_heur_conf_results(df: pd.DataFrame, outdir):
    filtered_df = df[~df['algorithm'].str.contains('DFS|CatDfs', case=False)]
    algConf = filtered_df[['algorithm', 'conf']].sort_values(['algorithm', 'conf'], ignore_index=True)
    algorithmGroups = algConf.groupby('algorithm')
    algConf.to_csv(outdir + 'heuristics_conf.csv', index=True, header=True)
    add_to_plot(field_name='conf', groups=algorithmGroups)
    create_figure(df=algConf, outdir=outdir, name="heuristics_conf",xlabel='Index', ylabel='Number of configurations')


def depth_first_configs(df: pd.DataFrame, outdir):
    filtered_df = df[df['algorithm'].str.contains('DFS|CatDfs|E', case=False)]
    algConf = filtered_df[['algorithm', 'conf']]
    filtered_df[['system', 'instance','algorithm', 'conf', 'makespan']].sort_values(['system', 'instance']).to_csv(outdir + 'dfs_conf.csv', index=False, header=True)
    algorithmGroups = algConf.groupby('algorithm')

    add_to_plot(field_name='conf', groups=algorithmGroups)
    create_figure(df=algConf, outdir=outdir, name="dfs_conf",xlabel='Index', ylabel='Number of configurations')


def depth_first_time(df: pd.DataFrame, outdir):
    filtered_df = df[df['algorithm'].str.contains('DFS|CatDfs|E', case=False)]
    algConf = filtered_df[['algorithm', 'time']]
    algConf.to_csv(outdir + 'dfs_time.csv', index=True, header=True)
    algorithmGroups = algConf.groupby('algorithm')

    add_to_plot(field_name='time', groups=algorithmGroups)
    create_figure(df=algConf, outdir=outdir, name="dfs_time",xlabel='Index', ylabel='Time (seconds)')

def create_data_table(df: pd.DataFrame, outdir):
    grouped = df.groupby(["system", "instance"])
    for (sysIns, data) in grouped:
        sys = data["system"].iloc[0]
        ins = data['instance'].iloc[0]
        key = sys + "-" + str(ins)
        out = outdir + "groups/"
        if not os.path.exists(out):
            os.makedirs(out)

        data[['algorithm', 'time', 'conf']].to_csv(out + key + '.csv', header=True, index=False)
    

def examine_result_files(directory, write_dir):
    print(directory, write_dir)
    data = []
    error_files = []
    for filename in os.listdir(directory):
        file_path = os.path.join(directory, filename)
        if filename.endswith('.out'):
            with open(file_path, 'r') as f:
                line = f.readline().strip()
                if line:
                    data.append(line.split(';'))
        elif filename.endswith('.err'):
            if os.path.getsize(file_path) > 0:
                error_files.append(file_path)

    # Save error summary to file
    with open(write_dir + 'error_summary.txt', 'w') as errorFile:
        with open(write_dir + 'verification_errors.txt', 'w') as verificationErrFile:
            for error_file in error_files:
                filetext = getFileText(error_file)
                if "At time " in filetext and "is broken.\n\t" in filetext and 'CAT-Verify' in filetext:
                    errorFile.write(f"{verificationErrFile}\n")
                    errorFile.write(f"{filetext}\n\n\n")
                else:
                    errorFile.write(f"{error_file}\n")
                    errorFile.write(f"{filetext}\n\n\n")

    return data

def generate_comparisons(value: pd.DataFrame, out_dir):
    value.to_csv(out_dir+'all.csv', index=False, header=True)
    generate_heur_time_results(value, out_dir)
    generate_heur_conf_results(value, out_dir)
    depth_first_configs(value, out_dir)
    depth_first_time(value, out_dir)



def create_only_completed_data(df: pd.DataFrame, outdir):   
    newOut = outdir + '/found_solutions/'
    print(df)
    value = df[(df["makespan"] != 2147483647)][(df["makespan"]!= -1)]
    if not os.path.exists(newOut):
        os.makedirs(newOut)

    generate_comparisons(value, newOut)


def write_no_found_schedules(df: pd.DataFrame, out_directory):
    err_dir = out_directory + '/no_solutions/'
    if not os.path.exists(err_dir):
        os.makedirs(err_dir)

    no_sols = df[(df["makespan"] == 2147483647) | (df["makespan"] < 0)]
    
    no_sols.to_csv(err_dir + 'no_solution.csv', index=False, header=True)
    algorithmGroups = no_sols.groupby('algorithm')

    instances = no_sols[['system','instance']].drop_duplicates()
    instances.to_csv(err_dir + 'problems_with_no_solutions.txt' , index=False, header=True)
    
    for algorithmName, group_data in algorithmGroups:
        data = group_data.sort_values('time', ignore_index=True)
        plt.plot(data['time'], label=algorithmName)

    create_figure(df=no_sols, outdir=err_dir, name="time",xlabel='Index', ylabel='Time (seconds)')

    for algorithmName, group_data in algorithmGroups:
        data = group_data.sort_values('conf', ignore_index=True)
        plt.plot(data['conf'], label=algorithmName)

    create_figure(df=no_sols, outdir=err_dir, name="configurations",xlabel='Index', ylabel='Number of configurations')




def process_directory(in_directory, out_directory):
    data = examine_result_files(in_directory, out_directory)
    df = create_initial_dataframe(data)
    df.to_csv(out_directory + 'combine.csv', index=False, header=True)
    generate_comparisons(df,out_directory)
   
    create_data_table(df, out_directory)
    create_only_completed_data(df, out_directory)
    write_no_found_schedules(df, out_directory)

def try_parse_to_full_column(file,outdir) -> pd.DataFrame:
    if file.endswith('.csv'):
        file = file
    else:
        file = file+'.csv'

    out = ''
    print('Making df for ' + file)
    if outdir.endswith('/'):
        out = outdir+file
    else:
        out = outdir + '/' + file

    try:
        with open(out, 'r') as csv_file:
            df = pd.read_csv(csv_file)
            df['error_text'] = 'None'
            return df
    except Exception:
        with open(out, 'r') as csv_file:
            df = pd.read_csv(csv_file)
            df['error_text'] = 'None'
            return df
        
def create_makespan(combined: pd.DataFrame, directory):
        makespan = combined[['algorithm', 'makespan']].sort_values(['algorithm', 'makespan'], ignore_index=True)
        makespan.to_csv(directory + 'makespan.csv', index=False, header=True)
        makespanGroups = makespan.groupby('algorithm')

        add_to_plot(makespanGroups, 'makespan')
        create_figure(df=makespan, outdir=directory+'/', name="makespan",xlabel='Index', ylabel='Make span')


def single_system(combined: pd.DataFrame, filter_str_exclude, directory, system):
    for_sys = combined[~combined['system'].str.contains(filter_str_exclude, case=False)]
    system_dir = directory+'/' +system
    if (not os.path.exists(system_dir)):
        os.makedirs(system_dir)
    create_makespan(combined=for_sys, directory=system_dir)
    depth_first_configs(outdir=system_dir + '/', df=for_sys)
    depth_first_time(outdir=system_dir + '/', df=for_sys)

def create_config(df: pd.DataFrame, outdir):
    algConf = df[['algorithm', 'conf']]
    df[['system', 'instance','algorithm', 'conf', 'makespan']].sort_values(['system', 'instance']).to_csv(outdir + 'conf.csv', index=False, header=True)
    algorithmGroups = algConf.groupby('algorithm')
    add_to_plot(algorithmGroups, 'conf')

    create_figure(df=algConf, outdir=outdir, name="conf",xlabel='Index', ylabel='Number of configurations')


def create_time(df: pd.DataFrame, outdir):
    algConf = df[['algorithm', 'time']]
    algConf.to_csv(outdir + 'time.csv', index=True, header=True)
    algorithmGroups = algConf.groupby('algorithm')
    add_to_plot(algorithmGroups, 'time')
    create_figure(df=algConf, outdir=outdir, name="time",xlabel='Index', ylabel='Time (seconds)')


def single_system_compare_heur_to_dfs(combined: pd.DataFrame, filter_sys_exclude, directory, subdir):
    combined = combined[~combined['algorithm'].str.contains('E', case=False)]
    for_sys = combined[~combined['system'].str.contains(filter_sys_exclude, case=False)]
    system_dir = directory+'/' + subdir
    if (not os.path.exists(system_dir)):
        os.makedirs(system_dir)

    create_makespan(combined=for_sys, directory=system_dir)
    create_config(df=for_sys, outdir=system_dir)
    create_time(df=for_sys, outdir=system_dir)

def analyse_all(data:pd.DataFrame, directory):
    create_makespan(combined=data, directory=directory)
    create_config(df=data, outdir=directory)
    create_time(df=data, outdir=directory)


def single_system_analyse(combined: pd.DataFrame, filter_sys_exclude, directory, subdir):
    for_sys = combined[~combined['system'].str.contains(filter_sys_exclude, case=False)]
    system_dir = directory+'/' + subdir
    if (not os.path.exists(system_dir)):
        os.makedirs(system_dir)

    create_makespan(combined=for_sys, directory=system_dir)
    create_config(df=for_sys, outdir=system_dir)
    create_time(df=for_sys, outdir=system_dir)


if __name__ == '__main__':
    if sys.argv[1] == 'analyse':
        directory =RESULT_DIR+ sys.argv[2]
        csv_file = sys.argv[3]
        combined = try_parse_to_full_column(file=csv_file, outdir=directory)
        combined = rename_fields(combined)
        #combined = combined[combined['time'] <= 600]
        combined = combined[combined['makespan'] > -1][combined['makespan'] < 2147483647][combined['time'] < 300]

        d = directory+'/'
        all = create_subdir(parent=d, dir_name='all')
        analyse_all(data=combined,directory=all)
        single_system_analyse(combined=combined, directory=all, subdir="S1/", filter_sys_exclude="S2|S3")
        single_system_analyse(combined=combined, directory=all, subdir="S2/", filter_sys_exclude="S1|S3")
        single_system_analyse(combined=combined, directory=all, subdir="S3/", filter_sys_exclude="S1|S2")


        
        dfs_comparisons_dir = d +'dfs_comparisons/'
        if (not os.path.exists(dfs_comparisons_dir)):
            os.makedirs(dfs_comparisons_dir)

        ## Only the different dfs searches
        dfs_data = combined[combined['algorithm'].str.contains('DFS|CatDfs|E|Cat*', case=False)][~combined['system'].str.contains('S1')]
        create_makespan(combined=dfs_data, directory=dfs_comparisons_dir)
        depth_first_configs(outdir=dfs_comparisons_dir, df=dfs_data)
        depth_first_time(outdir=dfs_comparisons_dir, df=dfs_data)
        

        data = combined[combined['algorithm'].str.contains('DFS|CatDfs|E|Cat*', case=False)][~combined['system'].str.contains('S1')]

        single_system_analyse(combined=data, directory=dfs_comparisons_dir, subdir="S1/", filter_sys_exclude="S2|S3")
        single_system_analyse(combined=data, directory=dfs_comparisons_dir, subdir="S2/", filter_sys_exclude="S1|S3")
        single_system_analyse(combined=data, directory=dfs_comparisons_dir, subdir="S3/", filter_sys_exclude="S1|S2")
#
        heur_comparison_dir = d + 'heur_dfs_comp/'
        if (not os.path.exists(heur_comparison_dir)):
           os.makedirs(heur_comparison_dir)
#
        ##only the heuristics and regular dfs
        data = combined[~combined['algorithm'].str.contains('E|CatDfs', case=False)]
        single_system_compare_heur_to_dfs(combined=data, directory=heur_comparison_dir, subdir="S1/", filter_sys_exclude="S2|S3")
        single_system_compare_heur_to_dfs(combined=data, directory=heur_comparison_dir, subdir="S2/", filter_sys_exclude="S1|S3")
        single_system_compare_heur_to_dfs(combined=data, directory=heur_comparison_dir, subdir="S3/", filter_sys_exclude="S1|S2")
#
        ##all 
        data = combined[~combined['algorithm'].str.contains('E', case=False)]
        single_system_compare_heur_to_dfs(combined=data, directory=heur_comparison_dir, subdir="all/S1/", filter_sys_exclude="S2|S3")
        single_system_compare_heur_to_dfs(combined=data, directory=heur_comparison_dir, subdir="all/S2/", filter_sys_exclude="S1|S3")
        single_system_compare_heur_to_dfs(combined=data, directory=heur_comparison_dir, subdir="all/S3/", filter_sys_exclude="S1|S2")

        system_dir = directory+'/' + 'only_heur/'
        if (not os.path.exists(system_dir)):
            os.makedirs(system_dir)

        data = combined[~combined['algorithm'].str.contains('E|CatDfs|DFS', case=False)]
        systems = data.groupby('system')
        for system, data in systems:
            data: pd.DataFrame = data
            print()
            sorted = data.sort_values(['instance'])
            sorted[['algorithm', 'system','instance', 'conf', 'time', 'makespan']].round({'time': 2}).sort_values(['instance', 'algorithm']).to_csv(system_dir+system+'_heristic.csv', index=False)

        

        create_makespan(combined=data, directory=system_dir)
        create_config(df=data, outdir=system_dir)
        create_time(df=data, outdir=system_dir)

    elif sys.argv[1] == 'csv_analyse':
        directory =RESULT_DIR+  sys.argv[2] + '/'
        print(directory)
        combined = pd.read_csv(directory + sys.argv[3])
        combined = combined[combined['makespan'] > -1][combined['makespan'] < 2147483647][combined['time'] <= 300]
        combined = rename_fields(combined=combined)

        heuristic_dir = create_subdir(directory,'heuristic')
        data = combined[combined['algorithm'].str.contains('Cat*', case=False)]
        analyse_all(data=data, directory=heuristic_dir)

        heurVsDfsDir = create_subdir(directory,'heuristicVsSDfs')
        data = combined
        analyse_all(data=data, directory=heurVsDfsDir)

        dfs_dir = create_subdir(directory,'all_dfs')
        data = combined[~combined['algorithm'].str.contains('Cat*', case=False)]
        analyse_all(data=data, directory=dfs_dir)

        end_result_dir = create_subdir(directory, "e5_rptt_dfs")
        
        end_data = combined[combined['algorithm'].str.contains('RPT-T|E = 5|DFS', case=False)][~combined['algorithm'].str.contains("MRPT")]
        analyse_all(data=end_data, directory=end_result_dir)

        s2_dir = create_subdir(end_result_dir, "S2")
        end_data_s2 = end_data[~end_data['system'].str.contains('S1|S3')]
        analyse_all(data=end_data_s2, directory=s2_dir)


        s3_dir = create_subdir(end_result_dir, "S3")
        end_data_s3 = end_data[~end_data['system'].str.contains('S1|S2')]
        analyse_all(data=end_data_s3, directory=s3_dir)

        
    elif sys.argv[1] == 'merge':
        print ('merging CSVs....')
        directory =RESULT_DIR+ sys.argv[2] + '/'
        csv_files = sys.argv[3:]
        frames = [try_parse_to_full_column(file=file, outdir=directory) for file in csv_files]
        combined = pd.concat(frames)
        combined.to_csv(directory + 'combine.csv', index=False, header=True)
        

    elif sys.argv[1] == 'csvmerge':
        print ('merging csvs....')
        directory =RESULT_DIR+ sys.argv[2]
        csv_files = sys.argv[3:]
        print(csv_files)

        frames = [try_parse_to_full_column(file=file, outdir=directory) for file in csv_files]
        combined = pd.concat(frames)
        combined = rename_fields(combined)
        #combined: pd.DataFrame = combined[combined['algorithm'].str.contains('DFS|CatDfs|E', case=False)]

        combined.to_csv(directory + '/combined.csv', index=False)
        combined = combined[combined['makespan'] > -1][combined['makespan'] < 2147483647]
    

        
        create_makespan(combined=combined, directory=directory+'/')
        depth_first_configs(outdir=directory+'/', df=combined)
        depth_first_time(outdir=directory+'/', df=combined)

        single_system(combined=combined, directory=directory+'/', filter_str_exclude='S1|S2', system="S3" )
        single_system(combined=combined, directory=directory+'/', filter_str_exclude='S1|S3', system="S2" )
        single_system(combined=combined, directory=directory+'/', filter_str_exclude='S2|S3', system="S1" )


    elif  sys.argv[1] == 'new':
        print('new')
        directory =RESULT_DIR + sys.argv[2] + '/'
        if not os.path.exists(directory):
                os.makedirs(directory)

        out_directory = directory + 'analysis/'
        if not os.path.exists(out_directory):
            os.makedirs(out_directory)


        data = examine_result_files(directory, out_directory)
        df = create_initial_dataframe_new(data)
        df.to_csv(out_directory + 'combine.csv', index=False, header=True)
        pass
    else:
        directories = [RESULT_DIR + experimentnumber for experimentnumber in sys.argv[1:]]
        ''' header for frame is ['algorithm', 'time']'''
        for in_directory in directories:
            write_dir = in_directory + "/" + 'analysis/'
            if not os.path.exists(write_dir):
                os.makedirs(write_dir)

            process_directory(in_directory, write_dir)



