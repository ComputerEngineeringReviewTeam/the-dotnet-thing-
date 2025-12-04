FROM debian:bookworm AS build
WORKDIR /src

RUN rm /bin/sh && ln -s /bin/bash /bin/sh

COPY main.py ./devices/
COPY requirements.txt ./devices/
COPY run_example.sh ./devices/

RUN apt update
RUN apt install python3 -y
RUN apt install python3-pip -y

RUN python3 -m pip install -r devices/requirements.txt --break-system-packages

ENTRYPOINT ["/bin/bash", "devices/run_example.sh"]
