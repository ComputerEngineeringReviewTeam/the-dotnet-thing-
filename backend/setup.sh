
sleep 10
node deploy.js

echo "Contract Address: "
cat ./contract.txt

dotnet backend.dll
