cd ..
reportgenerator -reports:TestResults/**/*.xml -targetdir:CoverageReport -reporttypes:Html -classfilters:-AspNetCoreGeneratedDocument*;-*.Migrations.*;-*ErrorViewModel;-*CartItemViewModel
echo Opening coverage report...
start chrome "file:///%cd:\=/%/CoverageReport/index.html"
cd webshopAI
