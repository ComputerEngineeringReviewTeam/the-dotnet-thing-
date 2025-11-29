trap "echo 'Stopping...'; kill 0" SIGINT

python3 devices/main.py --type=Salinity --id=1 --interval=1 &
python3 devices/main.py --type=Salinity --id=2 --interval=1 &
python3 devices/main.py --type=Salinity --id=3 --interval=1 &
python3 devices/main.py --type=Salinity --id=4 --interval=1 &
python3 devices/main.py --type=Temperature --id=5 --interval=1 &
python3 devices/main.py --type=Temperature --id=6 --interval=1 &
python3 devices/main.py --type=Temperature --id=7 --interval=1 &
python3 devices/main.py --type=Temperature --id=8 --interval=1 &
python3 devices/main.py --type=PH --id=9 --interval=1 &
python3 devices/main.py --type=PH --id=10 --interval=1 &
python3 devices/main.py --type=PH --id=11 --interval=1 &
python3 devices/main.py --type=PH --id=12 --interval=1 &
python3 devices/main.py --type=Oxygen --id=13 --interval=1 &
python3 devices/main.py --type=Oxygen --id=14 --interval=1 &
python3 devices/main.py --type=Oxygen --id=15 --interval=1 &
python3 devices/main.py --type=Oxygen --id=16 --interval=1 &

wait
