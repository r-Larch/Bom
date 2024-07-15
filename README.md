
![Build](https://github.com/r-Larch/Bom/workflows/Create%20Release/badge.svg)

# bom

A command-line tool to convert files to utf-8.

Use `--git` to quickly convert all files belonging to a git-repository while regard `.gitignore`.

```
cmd> bom --help

 @author: René Larch

 usage:  bom [ <directory> OPTIONS ]
  <directory>     if not specified current the working directory is used
  -r  --recurse   recursive list files
  -a  --all       show all files
  -n  --no-bom    show files without BOM
  -b  --bom       show files with BOM
  -c  --convert   convert files without BOM to uft-8
  -d  --add-bom   add a BOM when converting
  -g  --git       use .gitignore to filter files

 > bom -r -c -d *.cs

```