/* USE FROM POSTGRES DATABASE SQL WINDOW*/
CREATE USER monfoll WITH PASSWORD 'monfoll' SUPERUSER CREATEDB NOCREATEROLE NOREPLICATION;

/* CREATE A NEW CONNECTION WITH MONFOLL ROLE AND RUN FROM POSTGRES DATABASE */
CREATE DATABASE monfoll
  WITH OWNER = monfoll
       ENCODING = 'LATIN9'
       TABLESPACE = pg_default
       LC_COLLATE = 'C'
       LC_CTYPE = 'C'
       CONNECTION LIMIT = -1
       TEMPLATE = template0;

/* SET DATABASE TIMEZONE */
ALTER DATABASE monfoll
SET timezone TO 'us/mountain';