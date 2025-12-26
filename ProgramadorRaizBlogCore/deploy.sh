#!/bin/bash

# Configurações
USER="ubuntu"
HOST="arcus"
REMOTE_DIR="~/deploy_temp/programadorraiz.com.br/"
LOCAL_DIR="./bin/Release/net8.0/linux-x64/publish/"

echo "🚀 Iniciando Build..."
dotnet publish -c Release -r linux-x64 --self-contained

echo "📦 Sincronizando arquivos (apenas mudanças)..."
# Flags explicadas:
# -a: Archive (preserva permissões/datas)
# -v: Verbose (mostra detalhes)
# -z: Zip (comprime no envio)
# --progress: Mostra barra de progresso
rsync -avz --progress -e ssh $LOCAL_DIR $HOST:$REMOTE_DIR

echo "✅ Upload concluído!"