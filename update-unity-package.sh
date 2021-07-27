git checkout master # making sure that we are on the master branch before starting merge
git merge develop --no-ff

# getting the version number out of the "package.json"
# 1) finding the "package.json" file under "Assets"
# 2) printing the file's contents
# 3) getting the line containing the package's version
# 4) getting the actual version number (note that "sed" requires extended regex for "+" to be recognised, therefore we simply use "[0-9][0-9]*" to let it know we want minium one digit)
version=$(find Assets -name "package.json" | xargs cat | egrep "\"version\": " | sed 's/.*version.*\([0-9][0-9]*\.[0-9][0-9]*\.[0-9][0-9]*\).*/\1/')

git tag ${version}
git push --all
git push --tags
