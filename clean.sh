rm -rf out/production
rm -f net/pdfjet/*.exe.mdb
rm -f examples/*.exe.mdb
rm -f tests/*.exe.mdb
rm -f util/*.class
rm -f *.jar
rm -f *.exe
rm -f *.mdb
rm -f *.dll
rm -rf bin
rm -rf obj
rm -rf .build
rm -f *.pdf

for i in {1..50}
do
    if [ $i -lt 10 ]; then
        rm -rf examples/Example_0$i/bin
        rm -rf examples/Example_0$i/obj
    else
        rm -rf examples/Example_$i/bin
        rm -rf examples/Example_$i/obj
    fi
done
