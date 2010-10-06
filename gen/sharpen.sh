. `pwd`/config.make

DIRECTORY=$1
PROJECT=$2
SOURCES=$3
OPTIONS=`pwd`/$4
OUTDIR=`pwd`/$5
TEMPDIR=build

rm -rf $DIRECTORY/$TEMPDIR
mkdir -p $DIRECTORY/$TEMPDIR/$PROJECT
cp -r $DIRECTORY/$SOURCES $DIRECTORY/$TEMPDIR/$PROJECT
pushd $DIRECTORY
java \
		-Xms256m \
		-Xmx512m \
		-Declipse.consoleLog=true \
		-cp $ECLIPSE_PATH/plugins/org.eclipse.equinox.launcher_*.jar \
		org.eclipse.core.launcher.Main \
		-data $TEMPDIR \
		-application sharpen.core.application \
		$PROJECT/$SOURCES @$OPTIONS

popd
mkdir -p $OUTDIR
cp -r $DIRECTORY/$TEMPDIR/$PROJECT.net/$SOURCES/* $OUTDIR
