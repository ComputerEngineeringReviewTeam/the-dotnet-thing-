# Use official nginx image
FROM nginx:alpine

# Copy our custom nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy website files
COPY index.html /usr/share/nginx/html/index.html

COPY Delfin-butlonos-06.jpg /usr/share/nginx/html/Delfin-butlonos-06.jpg
