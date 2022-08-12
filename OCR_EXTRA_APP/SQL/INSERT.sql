
do $$
begin
    IF ((select count(*) from acte_ocr where id_acte = @id_acte) = 0) then

    insert into acte_ocr (@nom_de_champ) values (@valeur);

    END if;
end $$
